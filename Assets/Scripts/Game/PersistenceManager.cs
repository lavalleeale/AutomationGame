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
using static UnityEditor.Progress;

public class PersistenceManager : MonoBehaviour
{
    public WorldGenerationController worldGenController;
    public GameObject minerPrefab,
        conveyorPrefab,
        furnacePrefab,
        mergerPrefab,
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

    SavedProcessingBuilding[] ProcessingBuildingsToSave()
    {
        // Find all ProcessingBuildingBehaviour objects in the scene
        var buildings = FindObjectsOfType<ProcessingBuildingBehaviour>();

        // Create an array of SavedProcessingBuilding objects with the same length as the buildings array
        var output = new SavedProcessingBuilding[buildings.Length];

        for (int i = 0; i < buildings.Length; i++)
        {
            // Create a SavedItemStack array representing the inputs of the ProcessingBuildingBehaviour object at the current index
            var inputs = buildings[i].Processing
                .Select(
                    i => i == null ? null : new SavedItemStack(type: i.item.type, amount: i.amount)
                )
                .ToArray<SavedItemStack>();

            // Create a new SavedProcessingBuilding object at index i in the output array using the type, rotation, position, inputs, output, and currentRecipe of the ProcessingBuildingBehaviour object at the current index i
            output[i] = new SavedProcessingBuilding(
                type: buildings[i].SAVE_TYPE,
                rotation: (byte)(buildings[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    buildings[i].transform.position - buildings[i].Size * 0.32f
                ),
                inputs: inputs,
                output: buildings[i].Output == null
                    ? null
                    : new SavedItemStack(
                        type: buildings[i].Output.item.type,
                        amount: buildings[i].Output.amount
                    ),
                currentRecipe: Array.FindIndex(
                    buildings[i].recipes,
                    (recpie) => recpie == buildings[i].currentRecipe
                )
            );
        }

        // Return the array of SavedProcessingBuilding objects
        return output;
    }

    SavedBuilding[] BuildingsToSave()
    {
        // Find all BuildingBehaviour objects in the scene that are not ProcessingBuildingBehaviour
        var buildings = FindObjectsOfType<BuildingBehaviour>()
            .Where(x => !(x is ProcessingBuildingBehaviour))
            .ToArray();

        // Initialize an array to hold the saved building information
        var output = new SavedBuilding[buildings.Length];

        // Loop through each building and save its information
        for (int i = 0; i < buildings.Length; i++)
        {
            // Get the ConveyorController component attached to the building
            var controller = buildings[i].GetComponent<ConveyorController>();

            // Save the building's type, rotation, and position
            output[i] = new SavedBuilding(
                type: BuildingBehaviour.Type.conveyor,
                rotation: (byte)(buildings[i].transform.rotation.eulerAngles.z / 90),
                position: worldGenController.buildingGrid.WorldToCell(
                    buildings[i].transform.position - controller.Size * 0.32f
                )
            );
        }

        // Return the array of saved buildings
        return output;
    }

    SavedWorldItemStack[] ItemsToSave()
    {
        // Find all game objects with the "item" tag
        var items = GameObject.FindGameObjectsWithTag("item");

        // Initialize an array to hold the saved item information
        var output = new SavedWorldItemStack[items.Length];

        // Loop through each item and save its information
        for (int i = 0; i < items.Length; i++)
        {
            // Get the ItemStack component attached to the item
            var itemStack = items[i].GetComponent<ItemController>().itemStack;

            // Save the item's amount, type, and position
            output[i] = new SavedWorldItemStack(
                amount: itemStack.amount,
                type: itemStack.item.type,
                position: worldGenController.buildingGrid.WorldToCell(
                    items[i].transform.position - Vector3.one * 0.32f
                )
            );
        }

        // Return the array of saved items
        return output;
    }

    SavedOre[] OresToSave()
    {
        // Get a list of ore strength offsets from the WorldGenerationController
        var oreOffsets = WorldGenerationController.oreStrengthOffsets;

        // Convert the list of ore offsets into an array of SavedOre objects
        return oreOffsets.Select((ore) => new SavedOre(pos: ore.Key, offset: ore.Value)).ToArray();
    }

    SavedItemStack[] InventoryItemsToSave()
    {
        // Get a list of inventory items from the GameManager
        var items = GameManager.inventoryItems;

        // Convert the list of items into an array of SavedItemStack objects
        return items
            .Select(
                item =>
                    // Check if the item is null, and return null if it is
                    item == null
                        ? null
                        : new SavedItemStack(type: item.item.type, amount: item.amount)
            )
            .ToArray();
    }

    void Save()
    {
        // Create a new SaveData object with information about the buildings, processing buildings, items, chunks, and ores
        var data = new SaveData(
            buildings: BuildingsToSave(),
            processingBuildings: ProcessingBuildingsToSave(),
            items: ItemsToSave(),
            ores: OresToSave(),
            seed: WorldGenerationController.seed,
            inventory: InventoryItemsToSave(),
            favorites: new int?[10]
        );

        // Loop through the favorites list and save the index of each favorite building
        for (int i = 0; i < 10; i++)
        {
            // Find the index of the current favorite building in the list of building prefabs
            var index = Array.IndexOf(
                GameObject.Find("GameManager").GetComponent<PlacingController>().buildingPrefabs,
                PlacingController.FavoritesList[i]
            );

            // Save the index of the favorite building, or null if it is not found
            data.favorites[i] = index == -1 ? null : index;
        }

        // Create a save file at the specified location
        var saveLocation = Path.Combine(
            Application.persistentDataPath,
            $"{(saveName.text == "" ? DateTime.Now.ToString("yyyyMMddHHmmssffff") : saveName.text)}.dat"
        );
        using (FileStream file = File.Create(saveLocation))
        {
            // Serialize the SaveData object to the file using MessagePack
            MessagePackSerializer.Serialize(file, data);
        }

        // Log a message to indicate that the game data was saved successfully
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
        LoadOres(data.ores);

        worldGenController.Initialize(data.seed);

        LoadInventory(data.inventory);

        yield return null;

        LoadBuildings(data.buildings);

        LoadProcessingBuildings(data.processingBuildings);

        LoadItems(data.items);

        LoadFavorites(data.favorites);
    }

    void LoadFavorites(int?[] favorites)
    {
        // Get the PlacingController component attached to the GameManager game object
        var placingController = GameObject.Find("GameManager").GetComponent<PlacingController>();

        // Loop through the array of favorite indexes
        for (int i = 0; i < 10; i++)
        {
            // Check if the current index is not null
            if (favorites[i] != null)
            {
                // Get the building prefab at the specified index
                var prefab = placingController.buildingPrefabs[(int)favorites[i]];

                // Set the favorite at the current index to the building prefab
                placingController.SetFavorite(i, prefab);
            }
        }
    }

    void LoadOres(SavedOre[] ores)
    {
        // Loop through the array of SavedOre objects
        foreach (var ore in ores)
        {
            // Add an entry to the dictionary for the current ore, using its position as the key and its offset as the value
            WorldGenerationController.oreStrengthOffsets.Add(
                new Vector3Int(x: ore.x, y: ore.y),
                ore.offset
            );
        }
    }

    void LoadInventory(SavedItemStack[] items)
    {
        // Convert the array of SavedItemStack objects into an array of ItemStack objects
        GameManager.inventoryItems = items
            .Select(
                item =>
                    // Check if the item is null, and return null if it is
                    item == null
                        ? null
                        : new ItemStack(item: item.type.GetItem(), amount: item.amount)
            )
            .ToArray();
    }

    void LoadBuildings(SavedBuilding[] buildingsData)
    {
        foreach (var buildingData in buildingsData)
        {
            // Instantiate a new game object based on the type of building
            GameObject building;
            switch (buildingData.type)
            {
                case BuildingBehaviour.Type.miner:
                    building = Instantiate(minerPrefab);
                    break;
                case BuildingBehaviour.Type.conveyor:
                    building = Instantiate(conveyorPrefab);
                    break;
                case BuildingBehaviour.Type.merger:
                    building = Instantiate(mergerPrefab);
                    break;
                default:
                    throw new System.NotImplementedException("unknown building");
            }

            // Get the BuildingBehaviour component attached to the game object
            var behaviour = building.GetComponent<BuildingBehaviour>();

            // Set the position of the building based on its saved position
            building.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: buildingData.xPos, y: buildingData.yPos)
                )
                + behaviour.Size * 0.32f;

            // Set the rotation of the building based on its saved rotation
            building.transform.Rotate(xAngle: 0, yAngle: 0, zAngle: buildingData.rotation * 90);

            // Activate the building
            behaviour.Activate();
        }
    }

    void LoadItems(SavedWorldItemStack[] itemsData)
    {
        foreach (var itemData in itemsData)
        {
            // Instantiate a new item game object
            var item = Instantiate(itemPrefab);

            // Set the position of the item based on its saved position
            item.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: itemData.xPos, y: itemData.yPos)
                )
                + Vector3.one * 0.32f;

            // Get the ItemController component attached to the game object and set its item stack
            item.GetComponent<ItemController>().itemStack = new ItemStack(
                item: itemData.type.GetItem(),
                amount: itemData.amount
            );
        }
    }

    void LoadProcessingBuildings(SavedProcessingBuilding[] processingBuildingsData)
    {
        foreach (var processingBuildingData in processingBuildingsData)
        {
            // Instantiate a new game object based on the type of processing building
            GameObject building;
            switch (processingBuildingData.type)
            {
                case BuildingBehaviour.Type.furnace:
                    building = Instantiate(furnacePrefab);
                    break;
                case BuildingBehaviour.Type.constructor:
                    building = Instantiate(constructorPrefab);
                    break;
                case BuildingBehaviour.Type.assembler:
                    building = Instantiate(assemblerPrefab);
                    break;
                default:
                    throw new System.NotImplementedException("unknown building");
            }

            // Get the ProcessingBuildingBehaviour component attached to the game object
            var behaviour = building.GetComponent<ProcessingBuildingBehaviour>();

            // Set the position of the building based on its saved position
            building.transform.position =
                worldGenController.buildingGrid.CellToWorld(
                    new Vector3Int(x: processingBuildingData.xPos, y: processingBuildingData.yPos)
                )
                + behaviour.Size * 0.32f;

            // Set the rotation of the building based on its saved rotation
            building.transform.Rotate(
                xAngle: 0,
                yAngle: 0,
                zAngle: processingBuildingData.rotation * 90
            );

            // Activate the building
            behaviour.Activate();

            // Set the current recipe of the building, or null if no recipe is set
            behaviour.currentRecipe =
                processingBuildingData.currentRecipe == -1
                    ? null
                    : behaviour.recipes[processingBuildingData.currentRecipe];

            // Loop through the input items and add them to the building
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

            // Set the output item of the building, or null if no output is set
            if (processingBuildingData.output != null)
                behaviour.Output = new ItemStack(
                    item: processingBuildingData.output.type.GetItem(),
                    amount: processingBuildingData.output.amount
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

    [Key(4)]
    public SavedOre[] ores;

    [Key(5)]
    public int seed;

    [Key(6)]
    public SavedItemStack[] inventory;

    [Key(7)]
    public int?[] favorites;

    public SaveData(
        SavedBuilding[] buildings,
        SavedProcessingBuilding[] processingBuildings,
        SavedWorldItemStack[] items,
        SavedOre[] ores,
        int seed,
        SavedItemStack[] inventory,
        int?[] favorites
    )
    {
        this.buildings = buildings;
        this.processingBuildings = processingBuildings;
        this.items = items;
        this.ores = ores;
        this.seed = seed;
        this.inventory = inventory;
        this.favorites = favorites;
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
    public BuildingBehaviour.Type type;

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
        BuildingBehaviour.Type type,
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
        BuildingBehaviour.Type type,
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
}

[MessagePackObject]
public class SavedBuilding
{
    [Key(0)]
    public BuildingBehaviour.Type type;

    [Key(1)]
    public byte rotation;

    [Key(2)]
    public int xPos;

    [Key(3)]
    public int yPos;

    public SavedBuilding(BuildingBehaviour.Type type, byte rotation, Vector3Int position)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = position.x;
        this.yPos = position.y;
    }

    public SavedBuilding(BuildingBehaviour.Type type, byte rotation, int xPos, int yPos)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = xPos;
        this.yPos = yPos;
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
