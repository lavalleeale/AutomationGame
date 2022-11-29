using UnityEngine;
using System.Collections;

public class InventoryController : SlotController
{
    public GameObject slots,
        slotPrefab,
        itemPrefab;
    public Slot[] slotControllers;

    public override void DragFromSlot(SlotType type, int slotNum)
    {
        GameManager.inventoryItems[slotNum] = null;
    }

    public override bool DragToSlot(ItemStack itemStack, SlotType slotType, int slotNum)
    {
        if (GameManager.inventoryItems[slotNum] == null)
        {
            GameManager.inventoryItems[slotNum] = itemStack;
            return true;
        }
        return false;
    }

    public void Initialize()
    {
        slotControllers = new Slot[GameManager.inventoryItems.Length];
        for (int i = 0; i < slotControllers.Length; i++)
        {
            var slot = Instantiate(slotPrefab).GetComponent<Slot>();
            slot.Initialize(slotType: SlotType.input, i, this);
            if (GameManager.inventoryItems[i] != null)
            {
                var item = Instantiate(itemPrefab);
                item.GetComponent<UIItem>().itemStack = GameManager.inventoryItems[i];
                slot.Child = item;
            }
            slotControllers[i] = slot;
            slot.transform.SetParent(slots.transform, false);
        }
    }
}
