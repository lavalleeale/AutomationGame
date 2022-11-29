using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoadTest
{
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
            inventory: new SavedItemStack[30]
        );
        SceneManager.LoadScene("Menu");
        yield return null;
        PersistenceManager persistenceManager = GameObject
            .Find("Persistence Manager")
            .GetComponent<PersistenceManager>();
        SceneManager.LoadScene("Main");
        yield return null;
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
                new SavedBuilding(type: SavedBuilding.Type.miner, 1, new Vector3Int(-4, 0)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 1, new Vector3Int(-3, 0)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 1, new Vector3Int(0, 0)),
                new SavedBuilding(type: SavedBuilding.Type.miner, 0, new Vector3Int(3, 10)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(3, 9)),
                new SavedBuilding(type: SavedBuilding.Type.conveyor, 0, new Vector3Int(3, 6)),
            },
            processingBuildings: new SavedProcessingBuilding[] {
                new SavedProcessingBuilding(type: SavedProcessingBuilding.Type.furnace, 0, -2, 0, inputs: new SavedItemStack[1], output: null, currentRecipe: 0),
                new SavedProcessingBuilding(type: SavedProcessingBuilding.Type.furnace, 3, 3, 7, inputs: new SavedItemStack[1], output: null, currentRecipe: 1),
                new SavedProcessingBuilding(type: SavedProcessingBuilding.Type.constructor, 0, 1, 0, inputs: new SavedItemStack[1], output: null, currentRecipe: 1),
                new SavedProcessingBuilding(type: SavedProcessingBuilding.Type.constructor, 3, 3, 4, inputs: new SavedItemStack[1], output: null, currentRecipe: 2)
            },
            items: new SavedWorldItemStack[0],
            chunks: new LoadedChunk[1] { new LoadedChunk(0, 0) },
            ores: new SavedOre[0],
            seed: 0,
            inventory: new SavedItemStack[30]
        );
        SceneManager.LoadScene("Menu");
        yield return null;
        PersistenceManager persistenceManager = GameObject
            .Find("Persistence Manager")
            .GetComponent<PersistenceManager>();
        SceneManager.LoadScene("Main");
        yield return null;
        persistenceManager.PrepareMain();
        yield return persistenceManager.LoadData(data);

        Assert.IsNotNull(GameObject.FindGameObjectWithTag("furnace"));
        Assert.IsNull(GameObject.FindGameObjectWithTag("item"));
        yield return new WaitForSeconds(10);
        Assert.AreEqual(ItemCount(Item.Type.iron), 1);
        Assert.AreEqual(ItemCount(Item.Type.iron_plate), 1);
        Assert.AreEqual(ItemCount(Item.Type.copper), 1);
        Assert.AreEqual(ItemCount(Item.Type.copper_wire), 1);
    }

    int ItemCount(Item.Type type)
    {
        return GameObject.FindGameObjectsWithTag("item").Where(item => item.GetComponent<ItemController>().itemStack.item.type == type).Count();
    }
}
