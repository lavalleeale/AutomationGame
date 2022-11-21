using UnityEngine;
using System.Collections;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FurnaceController : ProcessingBuildingBehaviour
{
    protected override int MAX_INPUTS { get; set; } = 1;
    protected override int MAX_OUTPUTS { get; set; } = 1;
    protected override string NAME { get; set; } = "Furnace";
    Vector3 outputPos;
    float processingUntil = 0f;

    public GameObject itemPrefab;

    public RecipeScriptableObject[] recipes;
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
            if (Processing[0] == null)
            {
                var recipe = recipes.First(r => r.inputs[0].type == itemStack.item.type);
                if (recipe != null)
                {
                    currentRecipe = recipe;
                    Processing[0] = itemStack;
                    processingUntil = Time.time + recipe.processingTime;
                }
            }
            else if (
                Processing[0].item.type == itemStack.item.type
                && itemStack.amount + Processing[0].amount <= ItemStack.MAX_ITEMS
            )
            {
                Processing[0] = new ItemStack(
                    item: Processing[0].item,
                    amount: (byte)(Processing[0].amount + itemStack.amount)
                );
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Active && Processing[0] != null)
        {
            if (Time.time > processingUntil)
            {
                if (
                    Outputs[0]?.amount != ItemStack.MAX_ITEMS
                    && (Outputs[0] == null || Outputs[0].item.type == currentRecipe.outputs[0].type)
                )
                {
                    if (GUIController != null)
                    {
                        GUIController.UpdateProgress(0);
                    }
                    if (Outputs[0] == null)
                    {
                        Outputs[0] = new ItemStack(
                            item: currentRecipe.outputs[0].type.GetItem(),
                            amount: currentRecipe.outputs[0].amount
                        );
                    }
                    else
                    {
                        Outputs[0] = new ItemStack(
                            item: Outputs[0].item,
                            amount: (byte)(Outputs[0].amount + 1)
                        );
                    }
                    processingUntil = Time.time + currentRecipe.processingTime;

                    Processing[0] = new ItemStack(
                        item: Processing[0].item,
                        amount: (byte)(Processing[0].amount - 1)
                    );
                }
                var hit = Physics2D.OverlapBox(
                    point: outputPos,
                    size: Vector2.one * 0.32f,
                    angle: 0,
                    layerMask: outputMask
                );
                if (hit == null)
                {
                    var item = Instantiate(itemPrefab);
                    item.GetComponent<ItemController>().itemStack = new ItemStack(
                        item: Outputs[0].item,
                        amount: 1
                    );
                    item.transform.position = outputPos;
                    Outputs[0] = new ItemStack(
                        item: Outputs[0].item,
                        amount: (byte)(Outputs[0].amount - 1)
                    );
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
                    GUIController.UpdateProgress(
                        (currentRecipe.processingTime - (processingUntil - Time.time))
                            / currentRecipe.processingTime
                    );
                }
            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        var itemOffset =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0.96f, y: -0.32f, z: 0);
        outputPos = transform.position + itemOffset;
    }
}
