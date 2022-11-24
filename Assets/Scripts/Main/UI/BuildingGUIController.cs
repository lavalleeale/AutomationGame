﻿using UnityEngine;
using System.Collections;
using TMPro;

public class BuildingGUIController : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI nameText;
    public GameObject input,
        output,
        recipeContent,
        recipePrefab,
        slotPrefab,
        itemPrefab;
    public RectTransform processingForeground;
    ProcessingBuildingBehaviour building;
    RecipeScriptableObject[] recipes;
    Slot[] inputSlots;
    Slot outputSlot;

    void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("canvas").transform, false);
    }

    public void Initialize(
        string name,
        int inputCount,
        bool needsFuel,
        ProcessingBuildingBehaviour building,
        RecipeScriptableObject[] recipes
    )
    {
        nameText.text = name;
        inputSlots = new Slot[inputCount];
        for (int i = 0; i < inputCount; i++)
        {
            var slot = Instantiate(slotPrefab);
            slot.transform.SetParent(input.transform);
            inputSlots[i] = slot.GetComponent<Slot>();
            inputSlots[i].Initialize(SlotType.input, i, this);
        }

        this.outputSlot = output.GetComponent<Slot>();
        this.outputSlot.Initialize(SlotType.output, 0, this);
        for (int i = 0; i < recipes.Length; i++)
        {
            var recipeView = Instantiate(recipePrefab);
            recipeView.GetComponent<Recipe>().Initialize(recipes[i], this);
            recipeView.transform.SetParent(recipeContent.transform, false);
            recipeView.transform.position -= new Vector3(x: 0, y: i * 30);
        }
        var recipeContentTransform = recipeContent.GetComponent<RectTransform>();
        recipeContentTransform.sizeDelta = new Vector2(
            recipeContentTransform.sizeDelta.x,
            30 * recipes.Length
        );
        // TODO fuel
        this.building = building;
    }

    public void SelectRecipe(RecipeScriptableObject recipe)
    {
        building.SetRecipe(recipe);
    }

    public enum SlotType
    {
        input,
        output
    }

    public void SetSlot(SlotType slotType, int slotNum, ItemStack itemStack)
    {
        if (slotType == SlotType.input)
        {
            if (inputSlots[slotNum].Child)
            {
                Destroy(inputSlots[slotNum].Child);
            }
            if (itemStack != null)
            {
                var item = Instantiate(itemPrefab);
                item.GetComponent<UIItem>().itemStack = itemStack;
                inputSlots[slotNum].Child = item;
            }
        }
        else
        {
            if (outputSlot.Child)
            {
                Destroy(outputSlot.Child);
            }

            if (itemStack != null)
            {
                var item = Instantiate(itemPrefab);
                item.GetComponent<UIItem>().itemStack = itemStack;
                outputSlot.Child = item;
            }
        }
    }

    public void UpdateProgress(float amount)
    {
        processingForeground.offsetMax = new Vector2(
            -(1 - amount) * 190,
            processingForeground.offsetMax.y
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.inGUI = false;
            Destroy(gameObject);
        }
    }
}
