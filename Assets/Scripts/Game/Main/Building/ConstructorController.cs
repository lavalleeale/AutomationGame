using UnityEngine;
using System.Collections;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ConstructorController : ProcessingBuildingBehaviour
{
    protected override int MAX_INPUTS { get; set; } = 1;
    protected override string NAME { get; set; } = "Constructor";
    public override Vector3 Size { get; } = new Vector2(2, 2);
    public override Type SAVE_TYPE { get; set; } = Type.constructor;

    void Start()
    {
        outputMask = LayerMask.GetMask("items", "buildings");
    }

    public override void Activate()
    {
        base.Activate();
        var itemOffset =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0.96f, y: -0.32f, z: 0);
        outputPos = transform.position + itemOffset;
    }
}
