using UnityEngine;
using System.Collections;

public class FurnaceController : ProcessingBuildingBehaviour
{
    Vector3 outputPos;
    float processingUntil = 0f;
    bool processing = false;
    public GameObject itemPrefab;
    LayerMask outputMask;
    // TODO use recipes

    void Start()
    {
        Size = new Vector2(2, 2);
        outputMask = LayerMask.GetMask("items", "buildings");
    }

    public override bool CanInput(ItemStack itemStack)
    {
        return base.CanInput(itemStack) && itemStack.item.type == Item.Type.coal && !processing;
    }

    public override void Input(ItemStack itemStack)
    {
        processing = true;
        processingUntil = Time.time + 0.5f;
        return;
    }

    private void Update()
    {
        if (processing && Time.time > processingUntil)
        {
            var hit = Physics2D.OverlapBox(point: outputPos, size: Vector2.one * 0.32f, angle: 0, layerMask: outputMask);
            if (hit == null)
            {
                processing = false;
                var item = Instantiate(itemPrefab);
                item.GetComponent<ItemController>().itemStack = new ItemStack(item: Item.COAL, amount: 1);
                item.transform.position = outputPos;
            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        var itemOffset = Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward) * new Vector3(x: 0.96f, y: -0.32f, z: 0);
        outputPos = transform.position + itemOffset;
    }
}

