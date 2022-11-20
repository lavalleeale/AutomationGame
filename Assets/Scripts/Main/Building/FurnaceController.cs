using UnityEngine;
using System.Collections;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FurnaceController : ProcessingBuildingBehaviour
{
    Vector3 outputPos;
    float processingUntil = 0f;
    ItemStack processing;
    ItemStack Processing
    {
        set
        {
            if (value.amount == 0)
            {
                processing = null;
            }
            else
            {
                processing = value;
            }
            if (GUIController != null)
            {
                GUIController.SetSlot(BuildingGUIController.SlotType.input, 0, processing);
            }
        }
        get { return processing; }
    }
    ItemStack output;
    ItemStack Output
    {
        set
        {
            if (value.amount == 0)
            {
                output = null;
            }
            else
            {
                output = value;
            }
            if (GUIController != null)
            {
                GUIController.SetSlot(BuildingGUIController.SlotType.output, 0, output);
            }
        }
        get { return output; }
    }

    BuildingGUIController GUIController;
    public GameObject itemPrefab;
    public GameObject buildingGUIPrefab;

    public RecipeScriptableObject[] recipes;
    RecipeScriptableObject currentRecipe;
    LayerMask outputMask;
    // TODO use recipes

    void Start()
    {
        Size = new Vector2(2, 2);
        outputMask = LayerMask.GetMask("items", "buildings");
    }

    public override bool Input(ItemStack itemStack)
    {
        if (Active)
        {
            if (Processing == null)
            {
                var recipe = recipes.First(r => r.inputs[0].type == itemStack.item.type);
                if (recipe != null)
                {
                    currentRecipe = recipe;
                    Processing = itemStack;
                    processingUntil = Time.time + recipe.processingTime;
                }
            }
            else if (processing.item.type == itemStack.item.type && itemStack.amount + Processing.amount <= ItemStack.MAX_ITEMS)
            {
                Processing = new ItemStack(item: Processing.item, amount: (byte)(processing.amount + itemStack.amount));
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Processing != null)
        {
            if (Time.time > processingUntil)
            {
                if (output?.amount != ItemStack.MAX_ITEMS && (Output == null || Output.item.type == currentRecipe.outputs[0].type))
                {
                    if (GUIController != null)
                    {
                        GUIController.UpdateProgress(0);
                    }
                    if (Output == null)
                    {
                        Output = new ItemStack(item: currentRecipe.outputs[0].type.From(), amount: currentRecipe.outputs[0].amount);
                    }
                    else
                    {
                        Output = new ItemStack(item: Output.item, amount: (byte)(Output.amount + 1));
                    }
                    processingUntil = Time.time + currentRecipe.processingTime;

                    Processing = new ItemStack(item: Processing.item, amount: (byte)(Processing.amount - 1));


                    var hit = Physics2D.OverlapBox(point: outputPos, size: Vector2.one * 0.32f, angle: 0, layerMask: outputMask);
                    if (hit == null)
                    {
                        var item = Instantiate(itemPrefab);
                        item.GetComponent<ItemController>().itemStack = new ItemStack(item: Output.item, amount: 1);
                        item.transform.position = outputPos;
                        Output = new ItemStack(item: Output.item, amount: (byte)(Output.amount - 1));
                    }
                }
                else
                {
                    if (GUIController != null)
                    {
                        GUIController.UpdateProgress(1);
                    }
                }
            }
            else
            {
                if (GUIController != null)
                {
                    GUIController.UpdateProgress((currentRecipe.processingTime - (processingUntil - Time.time)) / currentRecipe.processingTime);
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (Active && GUIController == null)
        {
            var buildGUI = Instantiate(buildingGUIPrefab);
            GUIController = buildGUI.GetComponent<BuildingGUIController>();
            GUIController.Initialize("Furnace", 1, 1, true, this);
            GUIController.SetSlot(BuildingGUIController.SlotType.input, 0, Processing);
            GUIController.SetSlot(BuildingGUIController.SlotType.output, 0, Output);
        }
    }

    public override void Activate()
    {
        base.Activate();
        var itemOffset = Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward) * new Vector3(x: 0.96f, y: -0.32f, z: 0);
        outputPos = transform.position + itemOffset;
    }
}

