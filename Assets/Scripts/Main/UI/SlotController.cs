using UnityEngine;
using System.Collections;

public abstract class SlotController : MonoBehaviour
{
    public abstract bool DragToSlot(
        ItemStack itemStack,
        SlotController.SlotType slotType,
        int slotNum
    );
    public abstract void DragFromSlot(SlotType type, int slotNum);

    public enum SlotType
    {
        input,
        output
    }
}
