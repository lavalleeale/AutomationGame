using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : BuildingBehaviour
{
    public Vector3 pushDir;
    public override Vector3 Size { get; } = new Vector2(1, 1);
    public override Type SAVE_TYPE { get; set; } = Type.conveyor;

    public override void Activate()
    {
        base.Activate();
        pushDir =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0.1f, y: 0, z: 0);
    }
}
