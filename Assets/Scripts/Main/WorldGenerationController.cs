using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WorldGenerationController : MonoBehaviour
{
    public List<Vector3Int> knownChunks = new();
    public Vector3Int lastPos;
    public OreController[,,] oreControllers = new OreController[3, 3, 1024];
    public int[] oreControllersRowIndices = new int[] { 0, 1, 2 };
    public int[] oreControllersColIndices = new int[] { 0, 1, 2 };
    public GameObject orePrefab,
        ores,
        groundPrefab;
    public Grid buildingGrid,
        chunkGrid;
    bool active;
    public static int seed;
    public static Dictionary<Vector3Int, int> oreStrengthOffsets = new();

    public void LookAtCell(Vector3Int cell)
    {
        if (active)
        {
            if (cell == lastPos)
            {
                return;
            }
            switch (cell.x - lastPos.x, cell.y - lastPos.y)
            {
                case (0, 1):
                    for (int i = 0; i < 3; i++)
                    {
                        oreControllersRowIndices[i] = (oreControllersRowIndices[i] + 1) % 3;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i - 1, y: 1),
                            x: oreControllersColIndices[i],
                            y: oreControllersRowIndices[2]
                        );
                    }
                    break;
                case (0, -1):
                    for (int i = 0; i < 3; i++)
                    {
                        oreControllersRowIndices[i] = (oreControllersRowIndices[i] + 2) % 3;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i - 1, y: -1),
                            x: oreControllersColIndices[i],
                            y: oreControllersRowIndices[0]
                        );
                    }
                    break;
                case (1, 0):
                    for (int i = 0; i < 3; i++)
                    {
                        oreControllersColIndices[i] = (oreControllersColIndices[i] + 1) % 3;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: 1, y: i - 1),
                            x: oreControllersColIndices[2],
                            y: oreControllersRowIndices[i]
                        );
                    }
                    break;
                case (-1, 0):
                    for (int i = 0; i < 3; i++)
                    {
                        oreControllersColIndices[i] = (oreControllersColIndices[i] + 2) % 3;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: -1, y: i - 1),
                            x: oreControllersColIndices[0],
                            y: oreControllersRowIndices[i]
                        );
                    }
                    break;
                default:
                    Debug.Log("regen");
                    for (int i = 0; i < 9; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i % 3 - 1, y: i / 3 - 1),
                            x: oreControllersColIndices[i % 3],
                            y: oreControllersRowIndices[i / 3]
                        );
                    }
                    break;
            }
            lastPos = cell;
        }
    }

    public void Initialize(int seed)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int i = 0; i < 1024; i++)
                {
                    var ore = Instantiate(orePrefab);
                    ore.transform.SetParent(ores.transform);
                    oreControllers[x, y, i] = ore.GetComponent<OreController>();
                }
            }
        }
        this.active = true;
        WorldGenerationController.seed = seed;
        for (int i = 0; i < knownChunks.Count; i++)
        {
            var ground = Instantiate(groundPrefab);
            ground.transform.position = chunkGrid.CellToWorld(knownChunks[i]);
        }

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                Vector3Int cellPos = new Vector3Int(x: c - 1, y: r - 1);
                Generate(gridPos: cellPos, x: c, y: r);
            }
        }
    }

    public void Generate(Vector3Int gridPos, int x, int y)
    {
        spawnOre(centeredOn: gridPos, x: x, y: y);
    }

    void spawnOre(Vector3Int centeredOn, int x, int y)
    {
        int usedOres = 0;
        var buildingGridCenteredOn = centeredOn * 32;
        for (int r = 0; r < 32; r++)
        {
            for (int c = 0; c < 32; c++)
            {
                var pos = buildingGridCenteredOn;
                pos.x += c;
                pos.y += r;
                for (int i = 0; i < 3; i++)
                {
                    var strength = GetOreStrength(pos: pos, type: (OreController.Type)i);

                    if (strength > 0)
                    {
                        var controller = oreControllers[x, y, usedOres];
                        StartCoroutine(controller.Setup(type: (OreController.Type)i, pos: pos));
                        controller.transform.position =
                            buildingGrid.CellToWorld(pos) + new Vector3(x: 0.32f, y: 0.32f);
                        usedOres++;
                        break;
                    }
                }
            }
        }
    }

    public static int GetOreStrength(Vector3Int pos, OreController.Type type)
    {
        var strength = Mathf.PerlinNoise(
            pos.x / 64f + seed,
            pos.y / 64f + seed + 1000000 * (int)type
        );

        if (oreStrengthOffsets.TryGetValue(pos, out int value))
        {
            return (int)(1_000_000_000 * (strength - 0.9f) + 10_000) - value;
        }
        else
        {
            return (int)(1_000_000_000 * (strength - 0.9f) + 10_000);
        }
    }
}
