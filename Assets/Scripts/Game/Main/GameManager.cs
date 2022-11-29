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
    public static List<GUIType> inGUI = new();
    public static ItemStack[] inventoryItems = new ItemStack[13 * 4];
    public GameObject inventoryPrefab,
        canvas;
    InventoryController inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory == null)
            {
                if (!inGUI.Contains(GUIType.menu))
                {
                    inventory = Instantiate(inventoryPrefab).GetComponent<InventoryController>();
                    inventory.transform.SetParent(canvas.transform, false);
                    inventory.Initialize();
                    inGUI.Add(GUIType.inventory);
                }
            }
            else
            {
                inGUI.Remove(GUIType.inventory);
                Destroy(inventory.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inventory != null)
        {
            inGUI.Remove(GUIType.inventory);
            Destroy(inventory.gameObject);
        }
    }

    public enum GUIType
    {
        inventory,
        building,
        menu
    }
}
