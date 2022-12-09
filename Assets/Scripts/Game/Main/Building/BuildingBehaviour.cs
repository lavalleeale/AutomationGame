using System;
using UnityEngine;

public abstract class BuildingBehaviour : MonoBehaviour
{
    public bool Active { private set; get; }
    public abstract Vector3 Size { get; }
    public abstract Type SAVE_TYPE { get; set; }

    public virtual void Activate()
    {
        Active = true;
    }

    public virtual bool Input(ItemStack itemStack, string inputName)
    {
        return false;
    }

    private void OnMouseOver()
    {
        if (Active && UnityEngine.Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
        }
    }

    public enum Type : byte
    {
        miner,
        conveyor,
        furnace,
        constructor,
        assembler,
        merger
    }
}
