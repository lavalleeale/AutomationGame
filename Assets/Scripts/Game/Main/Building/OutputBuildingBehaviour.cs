using UnityEngine;
using System.Collections;

public abstract class OutputBuildingBehaviour : BuildingBehaviour
{
    public GameObject itemPrefab;
    protected Vector3 outputPos { get; set; }
    protected LayerMask outputMask;

    protected bool OutputItem(Item itemType)
    {
        var hit = Physics2D.OverlapBox(
            point: outputPos,
            size: Vector2.one * 0.32f,
            angle: 0,
            layerMask: outputMask
        );
        if (hit == null)
        {
            var item = Instantiate(itemPrefab);
            item.GetComponent<ItemController>().itemStack = new ItemStack(
                item: itemType,
                amount: 1
            );
            item.transform.position = outputPos;
            return true;
        }
        return false;
    }
}
