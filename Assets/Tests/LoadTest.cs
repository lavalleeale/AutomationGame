using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoadTest
{
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("Menu");
        yield return null;
        SceneManager.LoadScene("Main");
        yield return null;
    }

    [UnityTest]
    public IEnumerator MinerTest()
    {
        var data = new SaveData(
            buildings: new SavedBuilding[1]
            {
                new SavedBuilding(type: SavedBuilding.Type.miner, 1, new Vector3Int(-4, 0))
            },
            processingBuildings: new SavedProcessingBuilding[0],
            items: new SavedWorldItemStack[0],
            chunks: new LoadedChunk[1] { new LoadedChunk(0, 0) },
            ores: new SavedOre[0],
            seed: 0,
            inventory: new SavedItemStack[30],
            favorites: new int?[10]
        );
        PersistenceManager persistenceManager = GameObject
            .Find("Persistence Manager")
            .GetComponent<PersistenceManager>();
        persistenceManager.PrepareMain();
        yield return persistenceManager.LoadData(data);

        Assert.IsNotNull(GameObject.FindGameObjectWithTag("miner"));
        Assert.IsNull(GameObject.FindGameObjectWithTag("item"));
        yield return null;
        Assert.IsNotNull(GameObject.FindGameObjectWithTag("item"));
    }

    [UnityTest]
    public IEnumerator FactoryTest()
    {
        var data = new SaveData(
            buildings: new SavedBuilding[]
            {
                new SavedBuilding(type: SavedBuilding.Type.miner, 0, new Vector3Int(-4, 0)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(-3, 0)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(0, 0)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(3, 0)),
                new SavedBuilding(type: SavedBuilding.Type.miner, 3, new Vector3Int(3, 10)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 3, new Vector3Int(3, 9)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 3, new Vector3Int(3, 6)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 3, new Vector3Int(3, 3)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 3, new Vector3Int(3, 2)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(3, 1)),
            },
            processingBuildings: new SavedProcessingBuilding[]
            {
                new SavedProcessingBuilding(
                    type: SavedProcessingBuilding.Type.furnace,
                    0,
                    -2,
                    0,
                    inputs: new SavedItemStack[1],
                    output: null,
                    currentRecipe: 0
                ),
                new SavedProcessingBuilding(
                    type: SavedProcessingBuilding.Type.furnace,
                    3,
                    3,
                    7,
                    inputs: new SavedItemStack[1],
                    output: null,
                    currentRecipe: 1
                ),
                new SavedProcessingBuilding(
                    type: SavedProcessingBuilding.Type.constructor,
                    0,
                    1,
                    0,
                    inputs: new SavedItemStack[1],
                    output: null,
                    currentRecipe: 1
                ),
                new SavedProcessingBuilding(
                    type: SavedProcessingBuilding.Type.constructor,
                    3,
                    3,
                    4,
                    inputs: new SavedItemStack[1],
                    output: null,
                    currentRecipe: 2
                ),
                new SavedProcessingBuilding(
                    type: SavedProcessingBuilding.Type.assembler,
                    0,
                    4,
                    0,
                    inputs: new SavedItemStack[2],
                    output: null,
                    currentRecipe: 0
                ),
            },
            items: new SavedWorldItemStack[0],
            chunks: new LoadedChunk[1] { new LoadedChunk(0, 0) },
            ores: new SavedOre[0],
            seed: 0,
            inventory: new SavedItemStack[30],
            favorites: new int?[10]
        );
        PersistenceManager persistenceManager = GameObject
            .Find("Persistence Manager")
            .GetComponent<PersistenceManager>();
        persistenceManager.PrepareMain();
        yield return persistenceManager.LoadData(data);

        Assert.IsNull(GameObject.FindGameObjectWithTag("item"));
        yield return new WaitForSeconds(5);
        Assert.AreEqual(ItemCount(Item.Type.pcb), 1);
    }

    int ItemCount(Item.Type type)
    {
        return GameObject
            .FindGameObjectsWithTag("item")
            .Where(item => item.GetComponent<ItemController>().itemStack.item.type == type)
            .Count();
    }
}
