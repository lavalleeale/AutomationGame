using UnityEngine;
using System.Collections;

public class MergerController : BuildingBehaviour
{
    public override Vector3 Size { get; } = new Vector2(1, 1);

    Vector3 itemPos;
    ItemStack top,
        left,
        right;
    int nextOutput = 0;
    public GameObject itemPrefab;

    LayerMask spawnMask;

    void Start()
    {
        spawnMask = LayerMask.GetMask("items", "buildings");
    }

    public override bool Input(ItemStack itemStack, string inputName)
    {
        switch (inputName)
        {
            case "Input Top":
                if (top == null)
                {
                    top = itemStack;
                    return true;
                }
                break;
            case "Input Left":
                if (left == null)
                {
                    left = itemStack;
                    return true;
                }
                break;
            case "Input Right":
                if (right == null)
                {
                    right = itemStack;
                    return true;
                }
                break;
            default:
                throw new System.NotImplementedException("unkown input");
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            RaycastHit2D hit = Physics2D.Raycast(itemPos, Vector2.up, 0.1f, spawnMask);
            if (hit.collider == null)
            {
                if (nextOutput % 3 == 0)
                {
                    nextOutput++;
                    if (top != null)
                    {
                        var item = Instantiate(itemPrefab);
                        item.GetComponent<ItemController>().itemStack = top;
                        top = null;
                        item.transform.position = itemPos;
                        return;
                    }
                }
                if (nextOutput % 3 == 1)
                {
                    nextOutput++;
                    if (left != null)
                    {
                        var item = Instantiate(itemPrefab);
                        item.GetComponent<ItemController>().itemStack = left;
                        left = null;
                        item.transform.position = itemPos;
                        return;
                    }
                }
                if (nextOutput % 3 == 2)
                {
                    nextOutput++;
                    if (right != null)
                    {
                        var item = Instantiate(itemPrefab);
                        item.GetComponent<ItemController>().itemStack = right;
                        right = null;
                        item.transform.position = itemPos;
                        return;
                    }
                }
            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        var itemOffset =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0, y: -0.64f, z: 0);
        itemPos = transform.position + itemOffset;
    }
}
