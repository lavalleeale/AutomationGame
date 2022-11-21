using UnityEngine;
using System.Collections;

public class WorldGenerationController : MonoBehaviour
{
    public GameObject orePrefab;
    public GameObject ores;
    public Grid grid;
    int seed;

    public void Generate(int seed = 0)
    {
        this.seed = seed;
        spawnOre();
    }

    void spawnOre()
    {
        for (int r = 0; r < 100; r++)
        {
            for (int c = 0; c < 100; c++)
            {
                for (int i = 0; i < 3; i++)
                {
                    var strength = Mathf.PerlinNoise(c / 25f, r / 25f + seed + 1000000 * i);
                    if (strength > 0.8f)
                    {
                        var ore = Instantiate(orePrefab);
                        var controller = ore.GetComponent<OreController>();
                        controller.type = (OreController.Type)i;
                        controller.Strength = (int)(500000000 * (strength - 0.8f) + 10_000);
                        ore.transform.position =
                            grid.CellToWorld(new Vector3Int(x: c - 50, y: r - 50))
                            + new Vector3(x: 0.32f, y: 0.32f);
                        ore.transform.parent = ores.transform;
                    }
                }
            }
        }
    }
}
