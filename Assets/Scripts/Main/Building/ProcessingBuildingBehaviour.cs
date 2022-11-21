using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Specialized;

public abstract class ProcessingBuildingBehaviour : BuildingBehaviour
{
    protected abstract int MAX_INPUTS { get; set; }
    protected abstract int MAX_OUTPUTS { get; set; }
    protected abstract string NAME { get; set; }
    public abstract bool Input(ItemStack itemStack);
    public ObservableCollection<ItemStack> Processing;
    public ObservableCollection<ItemStack> Outputs;

    protected BuildingGUIController GUIController;
    public GameObject buildingGUIPrefab;

    public override void Activate()
    {
        base.Activate();
        Processing = new ObservableCollection<ItemStack>();
        for (int i = 0; i < MAX_INPUTS; i++)
        {
            Processing.Add(null);
        }

        Processing.CollectionChanged += HandleInputChange;

        Outputs = new ObservableCollection<ItemStack>();
        for (int i = 0; i < MAX_OUTPUTS; i++)
        {
            Outputs.Add(null);
        }

        Outputs.CollectionChanged += HandleOutputChange;
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
            GUIController.Initialize(NAME, MAX_INPUTS, MAX_OUTPUTS, true, this);
            for (int i = 0; i < MAX_INPUTS; i++)
            {
                GUIController.SetSlot(BuildingGUIController.SlotType.input, i, Processing[0]);
            }
            for (int i = 0; i < MAX_OUTPUTS; i++)
            {
                GUIController.SetSlot(BuildingGUIController.SlotType.output, i, Outputs[0]);
            }
        }
    }
}
