using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerationController : MonoBehaviour
{
    public List<Vector3Int> loadedChunks = new();
    public GameObject orePrefab,
        ores,
        groundPrefab;
    public Grid buildingGrid,
        chunkGrid;
    int seed;

    public void LookAtCell(Vector3Int cell)
    {
        for (int r = -5; r <= 5; r++)
        {
            for (int c = -5; c <= 5; c++)
            {
                var newCell = cell + new Vector3Int(x: c, y: r);
                if (!loadedChunks.Contains(newCell))
                {
                    var ground = Instantiate(groundPrefab);
                    ground.transform.position = chunkGrid.CellToWorld(newCell);
                    Generate(newCell);
                    loadedChunks.Add(newCell);
                }
            }
        }
    }

    public void Initialize(int seed)
    {
        this.seed = seed;
    }

    public void Generate(Vector3Int gridPos)
    {
        spawnOre(centeredOn: gridPos);
    }

    void spawnOre(Vector3Int centeredOn)
    {
        for (int r = 0; r < 32; r++)
        {
            for (int c = 0; c < 32; c++)
            {
                for (int i = 0; i < 3; i++)
                {
                    var strength = Mathf.PerlinNoise(
                        c / 32f + centeredOn.x + 1000000,
                        r / 32f + centeredOn.y + seed + 1000000 * (i + 1)
                    );
                    if (strength > 0.9f)
                    {
                        var ore = Instantiate(orePrefab);
                        var controller = ore.GetComponent<OreController>();
                        controller.type = (OreController.Type)i;
                        controller.Strength = (int)(1000000000 * (strength - 0.9f) + 10_000);
                        ore.transform.position =
                            buildingGrid.CellToWorld(
                                centeredOn * 32 + new Vector3Int(x: c - 16, y: r - 16)
                            ) + new Vector3(x: 0.32f, y: 0.32f);
                        ore.transform.parent = ores.transform;
                    }
                }
            }
        }
    }
}
