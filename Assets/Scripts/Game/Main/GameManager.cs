using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static List<GUIType> openGUIs = new();
    public static ItemStack[] inventoryItems = new ItemStack[10 * 5];
    public GameObject inventoryPrefab,
        canvas;
    InventoryController inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory == null)
            {
                if (!openGUIs.Contains(GUIType.menu))
                {
                    inventory = Instantiate(inventoryPrefab).GetComponent<InventoryController>();
                    inventory.transform.SetParent(canvas.transform, false);
                    inventory.Initialize();
                    openGUIs.Add(GUIType.inventory);
                }
            }
            else
            {
                openGUIs.Remove(GUIType.inventory);
                Destroy(inventory.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inventory != null)
        {
            openGUIs.Remove(GUIType.inventory);
            Destroy(inventory.gameObject);
        }
    }

    public static bool OnlyOpen(GUIType type)
    {
        return openGUIs.Count == 0 || (openGUIs.Count == 1 && openGUIs.Contains(type));
    }

    public static bool AnyOpen(params GUIType[] types)
    {
        foreach (var type in types)
        {
            if (openGUIs.Contains(type))
            {
                return true;
            }
        }
        return false;
    }   
}

public enum GUIType
{
    inventory,
    building,
    menu,
    placing
}