using MessagePack;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class PersistenceManager : MonoBehaviour
{
    public WorldGenerationController worldGenController;
    public GameObject minerPrefab,
        conveyorPrefab,
        furnacePrefab,
        itemPrefab,
        orePrefab,
        loadPrefab;
    public GameObject mainMenu,
        loadMenu,
        saveMenu;
    public Button loadButton;
    TMP_InputField saveName;
    string[] saveFiles;
    string toLoad;

    void Start()
    {
        var t = new DirectoryInfo(Application.persistentDataPath);
        saveFiles = t.GetFiles().Select(file => file.Name).ToArray();
        if (saveFiles.Length == 0)
        {
            loadButton.interactable = false;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void CreateWorld()
    {
        SceneManager.LoadScene("Main");
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    public void ShowLoadMenu()
    {
        mainMenu.SetActive(false);
        for (int i = 0; i < saveFiles.Length; i++)
        {
            var button = Instantiate(loadPrefab);
            button.transform.SetParent(loadMenu.transform.parent, false);
            button.transform.position = button.transform.position - new Vector3(x: 0, y: i * 30);
            button.GetComponentInChildren<TextMeshProUGUI>().text = saveFiles[i];
            button.GetComponent<Button>().onClick.AddListener(setToLoad(saveFiles[i]));
        }
    }

    UnityEngine.Events.UnityAction setToLoad(string target)
    {
        return () =>
        {
            toLoad = target;
            SceneManager.LoadScene("Main");
            SceneManager.activeSceneChanged += OnSceneLoaded;
        };
    }

    void OnSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name.Equals("Main"))
        {
            saveMenu = GameObject.Find("Save Menu");
            saveName = saveMenu.transform.Find("Save Name").GetComponent<TMP_InputField>();
            saveMenu.transform.Find("Save Button").GetComponent<Button>().onClick.AddListener(Save);
            saveMenu.SetActive(false);
            worldGenController = GameObject
                .Find("GameManager")
                .GetComponent<WorldGenerationController>();
            if (toLoad != null)
            {
                Load(toLoad);
            }
            else
            {
                worldGenController.Initialize(UnityEngine.Random.Range(0, 100000));
            }
        }
    }

    void Save()
    {
        var furnaces = GameObject.FindGameObjectsWithTag("furnace");
        var miners = GameObject.FindGameObjectsWithTag("miner");
        var conveyors = GameObject.FindGameObjectsWithTag("conveyor");
        var items = GameObject.FindGameObjectsWithTag("item");
        var ores = GameObject.FindGameObjectsWithTag("ore");

        var data = new SaveData(
            buildings: new SavedBuilding[furnaces.Length + miners.Length + conveyors.Length],
            items: new SavedWorldItemStack[items.Length],
            chunks: worldGenController.loadedChunks
                .Select(i => new LoadedChunk(x: i.x, y: i.y))
                .ToArray(),
            ores: new SavedOre[ores.Length],
            seed: worldGenController.seed
        );

        for (int i = 0; i < furnaces.Length; i++)
        {
            var controller = furnaces[i].GetComponent<FurnaceController>();
            var inputs = controller.Processing
                .Select(i => new SavedItemStack(type: i.item.type, amount: i.amount))
                .ToArray<SavedItemStack>();
            data.buildings[i] = new SavedBuilding(
                type: SavedBuilding.Type.furnace,
                rotation: (byte)(furnaces[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    furnaces[i].transform.position
                ),
                inputs: inputs,
                output: new SavedItemStack(
                    type: controller.Output.item.type,
                    amount: controller.Output.amount
                )
            );
        }

        for (int i = 0; i < miners.Length; i++)
        {
            data.buildings[i + furnaces.Length] = new SavedBuilding(
                type: SavedBuilding.Type.miner,
                rotation: (byte)(miners[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(miners[i].transform.position),
                inputs: null,
                output: null
            );
        }

        for (int i = 0; i < conveyors.Length; i++)
        {
            data.buildings[i + furnaces.Length + miners.Length] = new SavedBuilding(
                type: SavedBuilding.Type.conveyor,
                rotation: (byte)(conveyors[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    conveyors[i].transform.position
                ),
                inputs: null,
                output: null
            );
        }

        for (int i = 0; i < items.Length; i++)
        {
            var itemStack = items[i].GetComponent<ItemController>().itemStack;
            data.items[i] = new SavedWorldItemStack(
                amount: itemStack.amount,
                type: itemStack.item.type,
                position: worldGenController.buildingGrid.WorldToCell(items[i].transform.position)
            );
        }

        for (int i = 0; i < ores.Length; i++)
        {
            var ore = ores[i].GetComponent<OreController>();
            data.ores[i] = new SavedOre(
                pos: worldGenController.buildingGrid.WorldToCell(ore.transform.position),
                capacity: ore.Strength
            );
        }

        var saveLocation = Path.Combine(
            Application.persistentDataPath,
            $"{(saveName.text == "" ? DateTime.Now.ToString("yyyyMMddHHmmssffff") : saveName.text)}.dat"
        );
        using (FileStream file = File.Create(saveLocation))
        {
            MessagePackSerializer.Serialize(file, data);
        }

        Debug.Log($"Game data saved to {saveLocation}!");
    }

    public void Load(string saveName)
    {
        using FileStream file = File.OpenRead(
            Path.Combine(Application.persistentDataPath, saveName)
        );
        var data = MessagePackSerializer.Deserialize<SaveData>(file);
        foreach (var buildingData in data.buildings)
        {
            GameObject building;
            switch (buildingData.type)
            {
                case SavedBuilding.Type.furnace:
                    building = Instantiate(furnacePrefab);
                    var controller = building.GetComponent<FurnaceController>();
                    break;
                case SavedBuilding.Type.miner:
                    building = Instantiate(minerPrefab);
                    break;
                case SavedBuilding.Type.conveyor:
                    building = Instantiate(conveyorPrefab);
                    break;
                default:
                    throw new System.NotImplementedException("unknown building");
            }
            building.transform.position = new Vector3(x: buildingData.xPos, y: buildingData.yPos);

            building.transform.Rotate(xAngle: 0, yAngle: 0, zAngle: buildingData.rotation * 90);

            if (
                building.TryGetComponent<ProcessingBuildingBehaviour>(
                    out ProcessingBuildingBehaviour behaviour
                )
            )
            {
                behaviour.Activate();

                foreach (var input in buildingData.inputs)
                {
                    behaviour.Input(
                        new ItemStack(item: input.type.GetItem(), amount: input.amount),
                        ""
                    );
                }

                behaviour.Output = new ItemStack(
                    item: buildingData.output.type.GetItem(),
                    amount: buildingData.output.amount
                );
            }
            else
            {
                building.GetComponent<BuildingBehaviour>().Activate();
            }
        }
        foreach (var itemData in data.items)
        {
            var item = Instantiate(itemPrefab);
            item.transform.position = new Vector3(x: itemData.xPos, y: itemData.yPos);
            item.GetComponent<ItemController>().itemStack = new ItemStack(
                item: itemData.type.GetItem(),
                amount: itemData.amount
            );
        }

        foreach (var oreData in data.ores)
        {
            var ore = Instantiate(orePrefab);
            ore.transform.position = new Vector3(x: oreData.x, y: oreData.y);
            ore.GetComponent<OreController>().Strength = oreData.capacity;
        }

        worldGenController.loadedChunks = data.chunks
            .Select(i => new Vector3Int(x: i.x, y: i.y))
            .ToList();

        worldGenController.Initialize(data.seed);
    }
}

[MessagePackObject]
public class SaveData
{
    [Key(0)]
    public SavedBuilding[] buildings;

    [Key(1)]
    public SavedWorldItemStack[] items;

    [Key(2)]
    public LoadedChunk[] chunks;

    [Key(3)]
    public SavedOre[] ores;

    [Key(4)]
    public int seed;

    public SaveData(
        SavedBuilding[] buildings,
        SavedWorldItemStack[] items,
        LoadedChunk[] chunks,
        SavedOre[] ores,
        int seed
    )
    {
        this.buildings = buildings;
        this.items = items;
        this.chunks = chunks;
        this.ores = ores;
        this.seed = seed;
    }
}

[MessagePackObject]
public class SavedOre
{
    [Key(0)]
    public int x;

    [Key(1)]
    public int y;

    [Key(2)]
    public int capacity;

    public SavedOre(Vector3Int pos, int capacity)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.capacity = capacity;
    }

    public SavedOre(int x, int y, int capacity)
    {
        this.x = x;
        this.y = y;
        this.capacity = capacity;
    }
}

[MessagePackObject]
public class LoadedChunk
{
    [Key(0)]
    public int x;

    [Key(1)]
    public int y;

    public LoadedChunk(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[MessagePackObject]
public class SavedBuilding
{
    [Key(0)]
    public Type type;

    [Key(1)]
    public byte rotation;

    [Key(2)]
    public int xPos;

    [Key(3)]
    public int yPos;

    [Key(4)]
    public SavedItemStack[] inputs;

    [Key(5)]
    public SavedItemStack output;

    public SavedBuilding(
        Type type,
        byte rotation,
        Vector3Int position,
        SavedItemStack[] inputs,
        SavedItemStack output
    )
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = position.x;
        this.yPos = position.y;
        this.inputs = inputs;
        this.output = output;
    }

    public SavedBuilding(
        Type type,
        byte rotation,
        int xPos,
        int yPos,
        SavedItemStack[] inputs,
        SavedItemStack output
    )
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = xPos;
        this.yPos = yPos;
        this.inputs = inputs;
        this.output = output;
    }

    public enum Type : byte
    {
        furnace,
        miner,
        conveyor
    }
}

[MessagePackObject]
public class SavedWorldItemStack
{
    [Key(0)]
    public byte amount;

    [Key(1)]
    public Item.Type type;

    [Key(2)]
    public int xPos;

    [Key(3)]
    public int yPos;

    public SavedWorldItemStack(byte amount, Item.Type type, Vector3Int position)
    {
        this.amount = amount;
        this.type = type;
        this.xPos = position.x;
        this.yPos = position.y;
    }

    public SavedWorldItemStack(byte amount, Item.Type type, int xPos, int yPos)
    {
        this.amount = amount;
        this.type = type;
        this.xPos = xPos;
        this.yPos = yPos;
    }
}

[MessagePackObject]
public class SavedItemStack
{
    [Key(0)]
    public byte amount;

    [Key(1)]
    public Item.Type type;

    public SavedItemStack(byte amount, Item.Type type)
    {
        this.amount = amount;
        this.type = type;
    }
}
