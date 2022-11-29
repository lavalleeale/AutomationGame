using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : BuildingBehaviour
{
    public Vector3 pushDir;
    public override Vector3 Size { get; } = new Vector2(1, 1);

    public override void Activate()
    {
        base.Activate();
        pushDir =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0, y: -0.1f, z: 0);
    }
}
