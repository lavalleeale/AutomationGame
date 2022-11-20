using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    BuildingGUIController controller;
    BuildingGUIController.SlotType slotType;
    int slotNum;

    public GameObject Child
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
        set
        { value.transform.SetParent(transform); }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!Child)
        {
            Child = UIItem.itemBeingDragged;
        }
    }

    public void Initialize(BuildingGUIController.SlotType slotType, int slotNum, BuildingGUIController controller)
    {
        this.slotType = slotType;
        this.slotNum = slotNum;
        this.controller = controller;
    }
}
