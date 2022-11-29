using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;
using System;

public class MinerController : OutputBuildingBehaviour
{
    public override Vector3 Size { get; } = new Vector2(1, 1);
    List<OreData> oreData = new();
    float generationSpeed = 0.5f;
    float nextGeneration = 0;

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
        if (Active && oreData.Count > 0)
        {
            if (Time.time > nextGeneration)
            {
                nextGeneration = Time.time + generationSpeed;
                if (OutputItem(oreData[0].type.GetDrop()))
                {
                    oreData[0].amount -= 1;
                    WorldGenerationController.oreStrengthOffsets[oreData[0].pos] += 1;
                    if (oreData[0].amount == 0)
                    {
                        oreData.RemoveAt(0);
                    }
                }
            }
        }
    }

    public void FindOres()
    {
        var grid = GameObject.Find("Building Grid").GetComponent<Grid>();
        var pos = grid.WorldToCell(transform.position);
        WorldGenerationController.oreStrengthOffsets[pos] =
            WorldGenerationController.oreStrengthOffsets.GetValueOrDefault(pos);
        foreach (OreController.Type oreType in Enum.GetValues(typeof(OreController.Type)))
        {
            var strength = WorldGenerationController.GetOreStrength(pos, oreType);
            if (strength > 0)
            {
                oreData.Add(new OreData(pos: pos, type: oreType, amount: strength));
                break;
            }
        }
    }

    public override void Activate()
    {
        base.Activate();
        FindOres();
        var itemOffset =
            Quaternion.AngleAxis(transform.localRotation.eulerAngles.z, Vector3.forward)
            * new Vector3(x: 0, y: -0.64f, z: 0);
        outputPos = transform.position + itemOffset;
    }

    public class OreData
    {
        public Vector3Int pos;
        public int amount;
        public OreController.Type type;

        public OreData(Vector3Int pos, int amount, OreController.Type type)
        {
            this.pos = pos;
            this.amount = amount;
            this.type = type;
        }
    }
}
