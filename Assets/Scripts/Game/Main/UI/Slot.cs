using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    SlotController controller;
    SlotController.SlotType slotType;
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
        set { value.transform.SetParent(transform, false); }
    }

    public void OnClick()
    {
        if (!Child && controller.DragToSlot(UIItem.itemBeingDragged.itemStack, slotType, slotNum))
        {
            Child = UIItem.itemBeingDragged.gameObject;
            UIItem.itemBeingDragged.isBeingDragged = false;
            UIItem.itemBeingDragged.cg.blocksRaycasts = true;
            UIItem.itemBeingDragged = null;
        }
    }

    public bool Add()
    {
        return controller.DragToSlot(UIItem.itemBeingDragged.itemStack, slotType, slotNum);
    }

    public void Remove()
    {
        controller.DragFromSlot(slotType, slotNum);
    }

    public void Initialize(SlotController.SlotType slotType, int slotNum, SlotController controller)
    {
        this.slotType = slotType;
        this.slotNum = slotNum;
        this.controller = controller;
    }
}
