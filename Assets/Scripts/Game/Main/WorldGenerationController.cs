using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WorldGenerationController : MonoBehaviour
{
    public Vector3Int lastPos;
    public OreController[,,] oreControllers = new OreController[9, 9, 1024];
    int[] oreControllersRowIndices = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    int[] oreControllersColIndices = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
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
                    for (int i = 0; i < 9; i++)
                    {
                        oreControllersRowIndices[i] = (oreControllersRowIndices[i] + 1) % 9;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i - 4, y: 1),
                            x: oreControllersColIndices[i],
                            y: oreControllersRowIndices[8]
                        );
                    }
                    break;
                case (0, -1):
                    for (int i = 0; i < 9; i++)
                    {
                        oreControllersRowIndices[i] = (oreControllersRowIndices[i] + 2) % 9;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i - 4, y: -1),
                            x: oreControllersColIndices[i],
                            y: oreControllersRowIndices[0]
                        );
                    }
                    break;
                case (1, 0):
                    for (int i = 0; i < 9; i++)
                    {
                        oreControllersColIndices[i] = (oreControllersColIndices[i] + 1) % 9;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: 1, y: i - 4),
                            x: oreControllersColIndices[8],
                            y: oreControllersRowIndices[i]
                        );
                    }
                    break;
                case (-1, 0):
                    for (int i = 0; i < 9; i++)
                    {
                        oreControllersColIndices[i] = (oreControllersColIndices[i] + 2) % 9;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: -1, y: i - 4),
                            x: oreControllersColIndices[0],
                            y: oreControllersRowIndices[i]
                        );
                    }
                    break;
                default:
                    Debug.Log("regen");
                    for (int i = 0; i < 81; i++)
                    {
                        Generate(
                            gridPos: cell + new Vector3Int(x: i % 9 - 4, y: i / 9 - 4),
                            x: oreControllersColIndices[i % 9],
                            y: oreControllersRowIndices[i / 9]
                        );
                    }
                    break;
            }
            lastPos = cell;
        }
    }

    public void Initialize(int seed)
    {
        // Loop over each row and column of the ore grid.
        for (int y = 0; y < oreControllers.GetLength(0); y++)
        {
            for (int x = 0; x < oreControllers.GetLength(1); x++)
            {
                // Loop over each ore in the current row and column.
                for (int i = 0; i < 1024; i++)
                {
                    // Instantiate a new ore prefab.
                    var ore = Instantiate(orePrefab);

                    // Set the parent of the ore to the ores object.
                    ore.transform.SetParent(ores.transform);

                    // Store a reference to the ore controller in the oreControllers array.
                    oreControllers[x, y, i] = ore.GetComponent<OreController>();
                }
            }
        }

        // Set the active property to true and the seed property of the WorldGenerationController.
        this.active = true;
        WorldGenerationController.seed = seed;

        // Loop over each row and column of the chunk grid.
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                // Calculate the position of the current cell in the grid.
                Vector3Int cellPos = new Vector3Int(x: c - 4, y: r - 4);

                // Generate the chunk at the specified grid position.
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
        // Initialize the variable that will keep track of the number of used ores to 0.
        int usedOres = 0;

        // Calculate the position of the building grid centered on the specified position.
        var buildingGridCenteredOn = centeredOn * 32;

        // Loop over each row and column of the grid.
        for (int row = 0; row < 32; row++)
        {
            for (int column = 0; column < 32; column++)
            {
                // Calculate the current position in the grid by adding the row and column
                // offsets to the centered position of the building grid.
                var pos = buildingGridCenteredOn;
                pos.x += column;
                pos.y += row;

                // Loop over each type of ore.
                for (int oreType = 0; oreType < 3; oreType++)
                {
                    // Check the strength of the current ore at the specified position.
                    var strength = GetOreStrength(pos: pos, type: (OreController.Type)oreType);

                    // If the ore strength is greater than 0, spawn an ore object.
                    if (strength > 0)
                    {
                        // Get the ore controller from the oreControllers array at the specified indices.
                        var controller = oreControllers[x, y, usedOres];

                        // Start a coroutine to set up the ore controller with the specified type and position.
                        StartCoroutine(
                            controller.Setup(type: (OreController.Type)oreType, pos: pos)
                        );

                        // Set the position of the ore controller in the game world.
                        controller.transform.position =
                            buildingGrid.CellToWorld(pos) + new Vector3(x: 0.32f, y: 0.32f);

                        // Increment the number of used ores.
                        usedOres++;

                        // Break out of the loop.
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

#if !UNITY_EDITOR
        return (int)(500_000_000 * (strength - 0.8f) + 10_000);
#else
        return (int)(1_000_000_000 * (strength - 0.5f) + 10_000);
#endif
    }

    public static int GetOffsettedOreStrength(Vector3Int pos, OreController.Type type)
    {
        if (oreStrengthOffsets.TryGetValue(pos, out int value))
        {
            return GetOreStrength(pos, type) - value;
        }
        else
        {
            return GetOreStrength(pos, type);
        }
    }
}
