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
using UnityEngine.EventSystems;

public class PersistenceManager : MonoBehaviour
{
    public WorldGenerationController worldGenController;
    public GameObject minerPrefab,
        conveyorPrefab,
        furnacePrefab,
        constructorPrefab,
        assemblerPrefab,
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
        saveFiles = t.GetFiles("*.dat").Select(file => file.Name.Replace(".dat", "")).ToArray();
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
            button.transform.SetParent(loadMenu.transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = saveFiles[i];
            button.GetComponent<Button>().onClick.AddListener(setToLoad(saveFiles[i]));
        }
    }

    UnityEngine.Events.UnityAction setToLoad(string target)
    {
        return () =>
        {
            toLoad = $"{target}.dat";
            SceneManager.LoadScene("Main");
            SceneManager.activeSceneChanged += OnSceneLoaded;
        };
    }

    void OnSceneLoaded(Scene oldScene, Scene newScene)
    {
        if (newScene.name.Equals("Main"))
        {
            PrepareMain();
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

    public void PrepareMain()
    {
        saveMenu = GameObject.Find("Save Menu");
        saveName = saveMenu.transform.Find("Save Name").GetComponent<TMP_InputField>();
        saveMenu.transform.Find("Save Button").GetComponent<Button>().onClick.AddListener(Save);
        saveMenu.SetActive(false);
        worldGenController = GameObject
            .Find("GameManager")
            .GetComponent<WorldGenerationController>();
    }

    void Save()
    {
        var furnaces = GameObject.FindGameObjectsWithTag("furnace");
        var constructors = GameObject.FindGameObjectsWithTag("constructor");
        var miners = GameObject.FindGameObjectsWithTag("miner");
        var conveyors = GameObject.FindGameObjectsWithTag("conveyor");
        var items = GameObject.FindGameObjectsWithTag("item");

        var data = new SaveData(
            buildings: new SavedBuilding[miners.Length + conveyors.Length],
            processingBuildings: new SavedProcessingBuilding[furnaces.Length + constructors.Length],
            items: new SavedWorldItemStack[items.Length],
            chunks: worldGenController.knownChunks
                .Select(i => new LoadedChunk(x: i.x, y: i.y))
                .ToArray(),
            ores: WorldGenerationController.oreStrengthOffsets
                .Select((ore) => new SavedOre(pos: ore.Key, offset: ore.Value))
                .ToArray(),
            seed: WorldGenerationController.seed,
            inventory: GameManager.inventoryItems
                .Select(
                    item =>
                        item == null
                            ? null
                            : new SavedItemStack(type: item.item.type, amount: item.amount)
                )
                .ToArray()
        );

        for (int i = 0; i < furnaces.Length; i++)
        {
            var controller = furnaces[i].GetComponent<FurnaceController>();
            var inputs = controller.Processing
                .Select(
                    i => i == null ? null : new SavedItemStack(type: i.item.type, amount: i.amount)
                )
                .ToArray<SavedItemStack>();
            data.processingBuildings[i] = new SavedProcessingBuilding(
                type: SavedProcessingBuilding.Type.furnace,
                rotation: (byte)(furnaces[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    furnaces[i].transform.position - controller.Size * 0.32f
                ),
                inputs: inputs,
                output: controller.Output == null
                    ? null
                    : new SavedItemStack(
                        type: controller.Output.item.type,
                        amount: controller.Output.amount
                    ),
                currentRecipe: Array.FindIndex(
                    controller.recipes,
                    (recpie) => recpie == controller.currentRecipe
                )
            );
        }

        for (int i = 0; i < constructors.Length; i++)
        {
            var controller = constructors[i].GetComponent<ConstructorController>();
            var inputs = controller.Processing
                .Select(
                    i => i == null ? null : new SavedItemStack(type: i.item.type, amount: i.amount)
                )
                .ToArray<SavedItemStack>();
            data.processingBuildings[i + furnaces.Length] = new SavedProcessingBuilding(
                type: SavedProcessingBuilding.Type.constructor,
                rotation: (byte)(constructors[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    constructors[i].transform.position - controller.Size * 0.32f
                ),
                inputs: inputs,
                output: new SavedItemStack(
                    type: controller.Output.item.type,
                    amount: controller.Output.amount
                ),
                currentRecipe: Array.FindIndex(
                    controller.recipes,
                    (recpie) => recpie == controller.currentRecipe
                )
            );
        }

        for (int i = 0; i < miners.Length; i++)
        {
            var controller = miners[i].GetComponent<MinerController>();
            data.buildings[i] = new SavedBuilding(
                type: SavedBuilding.Type.miner,
                rotation: (byte)(miners[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    miners[i].transform.position - controller.Size * 0.32f
                )
            );
        }

        for (int i = 0; i < conveyors.Length; i++)
        {
            var controller = conveyors[i].GetComponent<ConveyorController>();
            data.buildings[i + miners.Length] = new SavedBuilding(
                type: SavedBuilding.Type.conveyor,
                rotation: (byte)(conveyors[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    conveyors[i].transform.position - controller.Size * 0.32f
                )
            );
        }

        for (int i = 0; i < items.Length; i++)
        {
            var itemStack = items[i].GetComponent<ItemController>().itemStack;
            data.items[i] = new SavedWorldItemStack(
                amount: itemStack.amount,
                type: itemStack.item.type,
                position: worldGenController.buildingGrid.WorldToCell(
                    items[i].transform.position - Vector3.one * 0.32f
                )
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
        StartCoroutine(LoadData(data: data));
    }

    public IEnumerator LoadData(SaveData data)
    {
        WorldGenerationController.oreStrengthOffsets = data.ores.ToDictionary(
            keySelector: (ore) => new Vector3Int(x: ore.x, y: ore.y),
            elementSelector: ore => ore.offset
        );

        worldGenController.knownChunks = data.chunks
            .Select(i => new Vector3Int(x: i.x, y: i.y))
            .ToList();

        worldGenController.Initialize(data.seed);

        GameManager.inventoryItems = data.inventory
            .Select(
                item =>
                    item == null
                        ? null
                        : new ItemStack(item: item.type.GetItem(), amount: item.amount)
            )
            .ToArray();
        yield return null;

        foreach (var buildingData in data.buildings)
        {
            GameObject building;
            switch (buildingData.type)
            {
                case SavedBuilding.Type.miner:
                    building = Instantiate(minerPrefab);
                    break;
                case SavedBuilding.Type.conveyor:
                    building = Instantiate(conveyorPrefab);
                    break;
                default:
                    throw new System.NotImplementedException("unknown building");
            }

            var behaviour = building.GetComponent<BuildingBehaviour>();
            building.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: buildingData.xPos, y: buildingData.yPos)
                )
                + behaviour.Size * 0.32f;

            building.transform.Rotate(xAngle: 0, yAngle: 0, zAngle: buildingData.rotation * 90);

            behaviour.Activate();
        }

        foreach (var processingBuildingData in data.processingBuildings)
        {
            GameObject building;
            switch (processingBuildingData.type)
            {
                case SavedProcessingBuilding.Type.furnace:
                    building = Instantiate(furnacePrefab);
                    break;
                case SavedProcessingBuilding.Type.constructor:
                    building = Instantiate(constructorPrefab);
                    break;
                case SavedProcessingBuilding.Type.assembler:
                    building = Instantiate(assemblerPrefab);
                    break;
                default:
                    throw new System.NotImplementedException("unknown building");
            }

            var behaviour = building.GetComponent<ProcessingBuildingBehaviour>();

            building.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: processingBuildingData.xPos, y: processingBuildingData.yPos)
                )
                + behaviour.Size * 0.32f;

            building.transform.Rotate(
                xAngle: 0,
                yAngle: 0,
                zAngle: processingBuildingData.rotation * 90
            );

            behaviour.Activate();

            behaviour.currentRecipe =
                processingBuildingData.currentRecipe == -1
                    ? null
                    : behaviour.recipes[processingBuildingData.currentRecipe];

            foreach (var input in processingBuildingData.inputs)
            {
                if (input != null)
                {
                    behaviour.Input(
                        new ItemStack(item: input.type.GetItem(), amount: input.amount),
                        ""
                    );
                }
            }

            if (processingBuildingData.output != null)
                behaviour.Output = new ItemStack(
                    item: processingBuildingData.output.type.GetItem(),
                    amount: processingBuildingData.output.amount
                );
        }

        foreach (var itemData in data.items)
        {
            var item = Instantiate(itemPrefab);
            item.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: itemData.xPos, y: itemData.yPos)
                )
                + Vector3.one * 0.32f;
            item.GetComponent<ItemController>().itemStack = new ItemStack(
                item: itemData.type.GetItem(),
                amount: itemData.amount
            );
        }
    }
}

[MessagePackObject]
public class SaveData
{
    [Key(0)]
    public SavedBuilding[] buildings;

    [Key(1)]
    public SavedProcessingBuilding[] processingBuildings;

    [Key(2)]
    public SavedWorldItemStack[] items;

    [Key(3)]
    public LoadedChunk[] chunks;

    [Key(4)]
    public SavedOre[] ores;

    [Key(5)]
    public int seed;

    [Key(6)]
    public SavedItemStack[] inventory;

    public SaveData(
        SavedBuilding[] buildings,
        SavedProcessingBuilding[] processingBuildings,
        SavedWorldItemStack[] items,
        LoadedChunk[] chunks,
        SavedOre[] ores,
        int seed,
        SavedItemStack[] inventory
    )
    {
        this.buildings = buildings;
        this.processingBuildings = processingBuildings;
        this.items = items;
        this.chunks = chunks;
        this.ores = ores;
        this.seed = seed;
        this.inventory = inventory;
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
    public int offset;

    public SavedOre(Vector3Int pos, int offset)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.offset = offset;
    }

    public SavedOre(int x, int y, int offset)
    {
        this.x = x;
        this.y = y;
        this.offset = offset;
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
public class SavedProcessingBuilding
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

    [Key(6)]
    public int currentRecipe;

    public SavedProcessingBuilding(
        Type type,
        byte rotation,
        Vector3Int position,
        SavedItemStack[] inputs,
        SavedItemStack output,
        int currentRecipe
    )
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = position.x;
        this.yPos = position.y;
        this.inputs = inputs;
        this.output = output;
        this.currentRecipe = currentRecipe;
    }

    public SavedProcessingBuilding(
        Type type,
        byte rotation,
        int xPos,
        int yPos,
        SavedItemStack[] inputs,
        SavedItemStack output,
        int currentRecipe
    )
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = xPos;
        this.yPos = yPos;
        this.inputs = inputs;
        this.output = output;
        this.currentRecipe = currentRecipe;
    }

    public enum Type : byte
    {
        furnace,
        constructor,
        assembler
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

    public SavedBuilding(Type type, byte rotation, Vector3Int position)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = position.x;
        this.yPos = position.y;
    }

    public SavedBuilding(Type type, byte rotation, int xPos, int yPos)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = xPos;
        this.yPos = yPos;
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
