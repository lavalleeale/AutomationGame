using UnityEngine;
using UnityEngine.U2D;

public class OreController : TooltipBehaviour
{
    public Sprite ironSprite;
    public Sprite coalSprite;
    public Sprite copperSprite;

    public bool Active = false;
    public int Strength
    {
        get { return WorldGenerationController.GetOreStrength(pos: pos, type: type); }
    }
    public Type type;
    public Vector3Int pos;

    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;

    public override string tooltipInfo => $"Amount: {Helpers.FormatNumber(Strength)}";

    // Start is called before the first frame update
    public void Setup(Type type, Vector3Int pos)
    {
        this.pos = pos;
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        this.type = type;
        switch (type)
        {
            case Type.coal:
                spriteRenderer.sprite = coalSprite;
                InitializeTooltip("Coal", coalSprite);
                break;
            case Type.copper:
                spriteRenderer.sprite = copperSprite;
                InitializeTooltip("Copper", copperSprite);
                break;
            case Type.iron:
                spriteRenderer.sprite = ironSprite;
                InitializeTooltip("Iron", ironSprite);
                break;
        }
        Active = true;
    }

    public void Deactivate()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        Active = false;
    }

    public enum Type
    {
        iron,
        coal,
        copper
    }
}

public static class OreExtensions
{
    public static Item GetDrop(this OreController.Type oreType)
    {
        switch (oreType)
        {
            case OreController.Type.iron:
                return Item.IRON_ORE;
            case OreController.Type.copper:
                return Item.COPPER_ORE;
            case OreController.Type.coal:
                return Item.COAL;
            default:
                throw new System.NotImplementedException("Unkown Item");
        }
    }
}
