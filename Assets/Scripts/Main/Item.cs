using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Item
{
    public static readonly Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
    public string name;
    public Type type;
    public Sprite sprite;

    public Item(string name, Type type, string spriteName)
    {
        this.name = name;
        this.type = type;
        this.sprite = sprites.Single(s => s.name == spriteName);
    }

    public static readonly Item COAL = new("Coal", Type.coal, "mined_coal");

    public static readonly Item IRON_ORE = new("Iron Ore", Type.iron_ore, "mined_iron");

    public static readonly Item COPPER_ORE = new("Copper Ore", Type.copper_ore, "mined_copper");

    public static readonly Item IRON = new("Iron", Type.iron, "ingot_iron");

    public static readonly Item COPPER = new("Copper", Type.copper, "ingot_copper");

    public enum Type: byte
    {
        coal, iron_ore, copper_ore, iron, copper
    }
}

public static class ItemExtensions
{
    public static Item From(this Item.Type itemType)
    {
        switch (itemType)
        {
            case Item.Type.coal:
                return Item.COAL;
            case Item.Type.iron_ore:
                return Item.IRON_ORE;
            case Item.Type.copper_ore:
                return Item.COPPER_ORE;
            case Item.Type.iron:
                return Item.IRON;
            case Item.Type.copper:
                return Item.COPPER;
            default:
                throw new System.NotImplementedException("Unkown Item");
        }
    }
}

public class ItemStack
{
    public static readonly int MAX_ITEMS = 5;
    public Item item;
    public byte amount;
    public ItemStack(Item item, byte amount)
    {
        this.item = item;
        this.amount = amount;
    }
}