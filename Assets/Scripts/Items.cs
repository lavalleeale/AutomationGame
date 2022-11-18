using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
    public Type type;

    public Item(string name, Type type)
    {
        this.name = name;
        this.type = type;
    }

    public static readonly Item COAL = new("Coal", Type.coal);
    public static readonly Item IRON_ORE = new("Iron Ore", Type.iron);
    public static readonly Item COPPER_ORE = new("Copper", Type.copper);

    public enum Type
    {
        coal, iron, copper
    }
}

public class ItemStack
{
    public Item item;
    public int amount;
    public ItemStack(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}