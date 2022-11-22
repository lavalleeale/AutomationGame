using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Specialized;
using System.Linq;

public abstract class ProcessingBuildingBehaviour : BuildingBehaviour
{
    protected abstract int MAX_INPUTS { get; set; }
    protected abstract string NAME { get; set; }

    public ObservableCollection<ItemStack> Processing;
    private ItemStack output;
    public ItemStack Output
    {
        get { return output; }
        set
        {
            if (GUIController != null)
            {
                GUIController.SetSlot(
                    BuildingGUIController.SlotType.output,
                    0,
                    value
                );
            }
            output = value;
        }
    }
    public RecipeScriptableObject[] recipes;
    public RecipeScriptableObject currentRecipe;
    protected float processingUntil = 0f;
    protected Vector3 outputPos { get; set; }
    protected LayerMask outputMask;
    public GameObject itemPrefab;

    protected BuildingGUIController GUIController;
    public GameObject buildingGUIPrefab;

    public override bool Input(ItemStack itemStack, string inputName)
    {
        if (Active)
        {
            // If item already in Processing add to item count and return true
            for (int i = 0; i < Processing.Count; i++)
            {
                if (
                    Processing[i]?.item.type == itemStack.item.type
                    && itemStack.amount + Processing[i].amount <= ItemStack.MAX_ITEMS
                )
                {
                    Processing[i] = new ItemStack(
                        item: Processing[i].item,
                        amount: (byte)(Processing[i].amount + itemStack.amount)
                    );
                    return true;
                }
            }

            if (currentRecipe == null)
            {
                // Get first recipe with all current processing items and new item
                var recipe = recipes.First(recipe =>
                {
                    foreach (var item in Processing.Where(item => item != null))
                    {
                        if (recipe.inputs.First(input => input.type == item.item.type) == null)
                            return false;
                    }
                    return recipe.inputs.First(input => input.type == itemStack.item.type) != null;
                });
                if (recipe != null)
                {
                    currentRecipe = recipe;
                    Processing[0] = itemStack;
                    processingUntil = Time.time + recipe.processingTime;
                }
            }
        }
        return false;
    }

    private void Update()
    {
        if (Active)
        {
            if (currentRecipe != null)
            {
                if (Time.time > processingUntil)
                {
                    // If output occupied but different item or would go over limit
                    if (
                        Output != null
                        && (
                            Output.item.type != currentRecipe.output.type
                            || Output.amount + currentRecipe.output.amount
                                > ItemStack.MAX_ITEMS
                        )
                    )
                    {
                        OutputItem();
                        return;
                    }

                    if (GUIController != null)
                    {
                        GUIController.UpdateProgress(0);
                    }

                    if (Output == null)
                    {
                        Output = new ItemStack(
                            item: currentRecipe.output.type.GetItem(),
                            amount: currentRecipe.output.amount
                        );
                    }
                    else
                    {
                        Output = new ItemStack(
                            item: Output.item,
                            amount: (byte)(Output.amount + currentRecipe.output.amount)
                        );
                    }
                    processingUntil = Time.time + currentRecipe.processingTime;


                    Processing[0] = new ItemStack(
                        item: Processing[0].item,
                        amount: (byte)(Processing[0].amount - 1)
                    );
                    OutputItem();
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
            else
            {
                currentRecipe = null;
            }
        }
    }

    void OutputItem()
    {
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
                item: Output.item,
                amount: 1
            );
            item.transform.position = outputPos;
            Output = new ItemStack(
                item: Output.item,
                amount: (byte)(Output.amount - 1)
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

    public override void Activate()
    {
        base.Activate();
        Processing = new ObservableCollection<ItemStack>();
        for (int i = 0; i < MAX_INPUTS; i++)
        {
            Processing.Add(null);
        }

        Processing.CollectionChanged += HandleInputChange;
    }

    private void HandleInputChange(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (GUIController != null)
        {
            GUIController.SetSlot(
                BuildingGUIController.SlotType.input,
                e.NewStartingIndex,
                (ItemStack)e.NewItems[0]
            );
        }
    }

    private void HandleOutputChange(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (GUIController != null)
        {
            GUIController.SetSlot(
                BuildingGUIController.SlotType.output,
                e.NewStartingIndex,
                (ItemStack)e.NewItems[0]
            );
        }
    }

    private void OnMouseDown()
    {
        if (Active && GUIController == null)
        {
            var buildGUI = Instantiate(buildingGUIPrefab);
            GUIController = buildGUI.GetComponent<BuildingGUIController>();
            GUIController.Initialize(NAME, MAX_INPUTS, true, this);
            for (int i = 0; i < MAX_INPUTS; i++)
            {
                GUIController.SetSlot(BuildingGUIController.SlotType.input, i, Processing[0]);
            }
            GUIController.SetSlot(BuildingGUIController.SlotType.output, 0, Output);
        }
    }
}
