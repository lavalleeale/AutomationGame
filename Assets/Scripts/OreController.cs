using UnityEngine;
using UnityEngine.U2D;

public class OreController : TooltipBehaviour
{
    public Sprite ironSprite;
    public Sprite coalSprite;
    public Sprite copperSprite;
    int strength;
    public int Strength { get { return strength; } set { UpdateTooltipInfo($"Amount: {Helpers.FormatNumber(value)}"); strength = value; } }

    public Type type;
    public Item drop;
    // Start is called before the first frame update
    void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        switch (type)
        {
            case Type.coal:
                drop = Item.COAL;
                spriteRenderer.sprite = coalSprite;
                InitializeTooltip("Coal", $"Amount: {Helpers.FormatNumber(strength)}", coalSprite);
                break;
            case Type.copper:
                drop = Item.COPPER_ORE;
                spriteRenderer.sprite = copperSprite;
                InitializeTooltip("Copper", $"Amount: {Helpers.FormatNumber(strength)}", copperSprite);
                break;
            case Type.iron:
                drop = Item.IRON_ORE;
                spriteRenderer.sprite = ironSprite;
                InitializeTooltip("Iron", $"Amount: {Helpers.FormatNumber(strength)}", ironSprite);
                break;
        }
    }

    public enum Type
    {
        iron, coal, copper
    }
}
