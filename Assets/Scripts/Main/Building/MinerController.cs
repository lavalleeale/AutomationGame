using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;

public class MinerController : BuildingBehaviour
{
    public override Vector3 Size { get; } = new Vector2(1, 1);
    public GameObject itemPrefab;
    List<OreController> ores = new();
    float generationSpeed = 0.5f;
    float nextGeneration = 0;
    Vector3 itemPos;

    LayerMask spawnMask;
    LayerMask oreMask;

    void Start()
    {
        spawnMask = LayerMask.GetMask("items", "buildings");
        oreMask = LayerMask.GetMask("ores");
    }

    // Update is called once per frame
    void Update()
    {
        if (Active && ores.Count > 0)
        {
            if (Time.time > nextGeneration)
            {
                nextGeneration = Time.time + generationSpeed;
                RaycastHit2D hit = Physics2D.Raycast(itemPos, Vector2.up, 0.1f, spawnMask);
                if (hit.collider == null)
                {
                    var item = Instantiate(itemPrefab);
                    item.GetComponent<ItemController>().itemStack = new ItemStack(
                        item: ores[0].drop,
                        amount: 1
                    );
                    ores[0].Strength--;
                    item.transform.position = itemPos;
                }
            }
        }
    }

    public void FindOres()
    {
        foreach (
            var ore in Physics2D.OverlapBoxAll(
                point: transform.position,
                size: new Vector2(x: 0.32f, y: 0.32f),
                angle: 0,
                layerMask: oreMask
            )
        )
        {
            ores.Add(ore.GetComponent<OreController>());
        }
    }

    public override void Activate()
    {
        base.Activate();
        Invoke(nameof(FindOres), 0);
        var itemOffset =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0, y: -0.64f, z: 0);
        itemPos = transform.position + itemOffset;
    }
}
