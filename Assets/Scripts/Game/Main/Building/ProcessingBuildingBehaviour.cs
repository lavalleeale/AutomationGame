﻿using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine.EventSystems;
using static BuildingGUIController;

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
            if (value == null || value.amount == 0)
            {
                output = null;
            }
            else
            {
                output = value;
            }
            if (GUIController != null)
            {
                GUIController.SetSlot(SlotController.SlotType.output, 0, output);
            }
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

    public void SetRecipe(RecipeScriptableObject newRecipe)
    {
        this.currentRecipe = newRecipe;
    }

    public override bool Input(ItemStack itemStack, string inputName)
    {
        if (Active && currentRecipe != null)
        {
            // If item already in Processing add to item count and return true
            for (int i = 0; i < Processing.Count; i++)
            {
                if (Processing[i]?.item.type == itemStack.item.type)
                {
                    if (itemStack.amount + Processing[i].amount <= ItemStack.MAX_ITEMS)
                    {
                        Processing[i] = new ItemStack(
                            item: Processing[i].item,
                            amount: (byte)(Processing[i].amount + itemStack.amount)
                        );
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            var firstNull = Processing.IndexOf(null);
            if (firstNull != -1)
            {
                foreach (RecipeScriptableObject.RecipeItem input in currentRecipe.inputs)
                {
                    if (input.type == itemStack.item.type)
                    {
                        Processing[firstNull] = itemStack;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool InputToSlot(ItemStack itemStack, SlotController.SlotType slotType, int slotNum)
    {
        if (currentRecipe)
        {
            if (slotType == SlotController.SlotType.input)
            {
                // If target slot is empty and processing does not already contain item and current recipe contains item
                if (
                    Processing[slotNum] == null
                    && Processing.FirstOrDefault(item => item?.item.type == itemStack.item.type)
                        == null
                    && currentRecipe.inputs.FirstOrDefault(
                        input => input.type == itemStack.item.type
                    ) != null
                )
                {
                    Processing[slotNum] = itemStack;
                }
                else if (
                    Processing[slotNum].item.type == itemStack.item.type
                    && Processing[slotNum].amount + itemStack.amount < ItemStack.MAX_ITEMS
                )
                {
                    Processing[slotNum] = new ItemStack(
                        item: itemStack.item,
                        amount: (byte)(Processing[slotNum].amount + itemStack.amount)
                    );
                    return true;
                }
            }
        }

        return false;
    }

    public void RemoveFromSlot(SlotController.SlotType type, int slotNum)
    {
        if (type == SlotController.SlotType.input)
        {
            Processing[slotNum] = null;
        }
        else
        {
            Output = null;
        }
    }

    private void Update()
    {
        if (Active)
        {
            if (Output != null && OutputItem())
            {
                return;
            }
            if (currentRecipe != null)
            {
                if (GUIController != null)
                {
                    GUIController.UpdateProgress(
                        Mathf.Clamp((currentRecipe.processingTime - (processingUntil - Time.time))
                            / currentRecipe.processingTime, 0, 1)
                    );
                }
                foreach (var input in currentRecipe.inputs)
                {
                    if (Processing.FirstOrDefault(item=>item?.item.type == input.type && item.amount > input.amount) == null)
                    {
                        return;
                    }
                }
                if (Time.time > processingUntil)
                {
                    // If output occupied but different item or would go over limit
                    if (
                        Output != null
                        && (
                            Output.item.type != currentRecipe.output.type
                            || Output.amount + currentRecipe.output.amount > ItemStack.MAX_ITEMS
                        )
                    )
                    {
                        return;
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

                    for (int i = 0; i < currentRecipe.inputs.Length; i++)
                    {
                        Processing[i] = new ItemStack(
                            item: Processing[i].item,
                            amount: (byte)(Processing[i].amount - currentRecipe.inputs[i].amount)
                        );
                    }
                }
            }
            else
            {
                if (GUIController != null)
                {
                    GUIController.UpdateProgress(0);
                }
            }
        }
    }

    bool OutputItem()
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
            Output = new ItemStack(item: Output.item, amount: (byte)(Output.amount - 1));
            return true;
        }
        return false;
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
        if (Processing[e.NewStartingIndex] == null)
        {
            processingUntil = Time.time + currentRecipe.processingTime;
        }
        if (e.NewItems[0] != null && ((ItemStack)e.NewItems[0]).amount == 0)
        {
            Processing[e.NewStartingIndex] = null;
            return;
        }
        if (GUIController != null)
        {
            GUIController.SetSlot(
                SlotController.SlotType.input,
                e.NewStartingIndex,
                (ItemStack)e.NewItems[0]
            );
        }
    }

    private void OnMouseDown()
    {
        if (
            Active
            && GUIController == null
            && !(
                GameManager.inGUI.Contains(GameManager.GUIType.menu)
                || GameManager.inGUI.Contains(GameManager.GUIType.building)
            )
        )
        {
            GameManager.inGUI.Add(GameManager.GUIType.building);
            var buildGUI = Instantiate(buildingGUIPrefab);
            GUIController = buildGUI.GetComponent<BuildingGUIController>();
            GUIController.Initialize(NAME, MAX_INPUTS, true, this, recipes);
            for (int i = 0; i < MAX_INPUTS; i++)
            {
                GUIController.SetSlot(SlotController.SlotType.input, i, Processing[i]);
            }
            GUIController.SetSlot(SlotController.SlotType.output, 0, Output);
        }
    }
}
