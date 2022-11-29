using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Recipe : MonoBehaviour
{
    public GameObject inputs,
        output,
        UIItemPrefab;
    public RecipeScriptableObject recipe;
    public BuildingGUIController controller;

    public void Initialize(RecipeScriptableObject recipe, BuildingGUIController controller)
    {
        this.controller = controller;
        this.recipe = recipe;
        foreach (var input in recipe.inputs)
        {
            var item = Instantiate(UIItemPrefab);
            var itemController = item.GetComponent<UIItem>();
            itemController.itemStack = new ItemStack(input.type.GetItem(), input.amount);
            itemController.CanDrag = false;
            item.transform.SetParent(inputs.transform, false);
        }
        var outputItem = Instantiate(UIItemPrefab);
        var outputItemController = outputItem.GetComponent<UIItem>();
        outputItemController.itemStack = new ItemStack(
            recipe.output.type.GetItem(),
            recipe.output.amount
        );
        outputItemController.CanDrag = false;
        outputItem.transform.SetParent(output.transform, false);
    }

    public void OnClick()
    {
        controller.SelectRecipe(recipe);
    }
}
