using System;
using UnityEngine;

public abstract class BuildingBehaviour : MonoBehaviour
{
    public bool Active { private set; get; }
    public abstract Vector3 Size { get; }

    public virtual void Activate()
    {
        Active = true;
    }

    public virtual bool Input(ItemStack itemStack, string inputName)
    {
        return false;
    }
}
