using System;
using UnityEngine;

public abstract class BuildingBehaviour : MonoBehaviour
{
    public bool Active { private set; get; }
    public Vector3 Size { protected set; get; }

    public virtual void Activate()
    {
        Active = true;
    }
}

