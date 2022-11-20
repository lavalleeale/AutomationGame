using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WorldGenerationController worldGenController;
    public GameObject minerPrefab, conveyorPrefab, furnacePrefab, itemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        worldGenController.Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var furnaces = GameObject.FindGameObjectsWithTag("furnace");
            var miners = GameObject.FindGameObjectsWithTag("miner");
            var conveyors = GameObject.FindGameObjectsWithTag("conveyor");
            var items = GameObject.FindGameObjectsWithTag("item");

            var data = new SaveData(buildings: new SavedBuilding[furnaces.Length + miners.Length + conveyors.Length], items: new SavedItemStack[items.Length]);

            for (int i = 0; i < furnaces.Length; i++)
            {
                data.buildings[i] = new SavedBuilding(
                    type: SavedBuilding.Type.furnace,
                    rotation: (byte)(furnaces[i].transform.rotation.eulerAngles.z / 90),
                    position: furnaces[i].transform.position);
            }

            for (int i = 0; i < miners.Length; i++)
            {
                data.buildings[i + furnaces.Length] = new SavedBuilding(
                    type: SavedBuilding.Type.miner,
                    rotation: (byte)(miners[i].transform.rotation.eulerAngles.z / 90),
                    position: miners[i].transform.position);
            }

            for (int i = 0; i < conveyors.Length; i++)
            {
                data.buildings[i + furnaces.Length + miners.Length] = new SavedBuilding(
                    type: SavedBuilding.Type.conveyor,
                    rotation: (byte)(conveyors[i].transform.rotation.eulerAngles.z / 90),
                    position: conveyors[i].transform.position);
            }


            for (int i = 0; i < items.Length; i++)
            {
                var itemStack = items[i].GetComponent<ItemController>().itemStack;
                data.items[i] = new SavedItemStack(amount: itemStack.amount, type: itemStack.item.type, position: items[i].transform.position);
            }

            using (FileStream file = File.Create(Application.persistentDataPath + "/save.dat"))
            {
                MessagePackSerializer.Serialize(file, data);
            }
            Debug.Log($"Game data saved to {Application.persistentDataPath + "/save.dat"}!");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            using (FileStream file = File.OpenRead(Application.persistentDataPath + "/save.dat"))
            {
                var data = MessagePackSerializer.Deserialize<SaveData>(file);
                foreach (var buildingData in data.buildings)
                {
                    GameObject building;
                    switch (buildingData.type)
                    {
                        case SavedBuilding.Type.furnace:
                            building = Instantiate(furnacePrefab);
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
                    building.GetComponent<BuildingBehaviour>().Activate();
                }
                foreach (var itemData in data.items)
                {
                    var item = Instantiate(itemPrefab);
                    item.transform.position = new Vector3(x: itemData.xPos, y: itemData.yPos);
                    item.GetComponent<ItemController>().itemStack = new ItemStack(item: itemData.type.From(), amount: itemData.amount);
                }
            }
        }
    }
}

[MessagePackObject]
public class SaveData
{
    [Key(0)]
    public SavedBuilding[] buildings;
    [Key(1)]
    public SavedItemStack[] items;

    public SaveData(SavedBuilding[] buildings, SavedItemStack[] items)
    {
        this.buildings = buildings;
        this.items = items;
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
    public float xPos;
    [Key(3)]
    public float yPos;

    public SavedBuilding(Type type, byte rotation, Vector2 position)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = position.x;
        this.yPos = position.y;
    }

    [SerializationConstructor]
    public SavedBuilding(Type type, byte rotation, float xPos, float yPos)
    {
        this.type = type;
        this.rotation = rotation;
        this.xPos = xPos;
        this.yPos = yPos;
    }

    public enum Type : byte
    {
        furnace, miner, conveyor
    }
}

[MessagePackObject]
public class SavedItemStack
{
    [Key(0)]
    public byte amount;
    [Key(1)]
    public Item.Type type;
    [Key(2)]
    public float xPos;
    [Key(3)]
    public float yPos;

    public SavedItemStack(byte amount, Item.Type type, Vector2 position)
    {
        this.amount = amount;
        this.type = type;
        this.xPos = position.x;
        this.yPos = position.y;
    }

    public SavedItemStack(byte amount, Item.Type type, float xPos, float yPos)
    {
        this.amount = amount;
        this.type = type;
        this.xPos = xPos;
        this.yPos = yPos;
    }
}