using Godot;

namespace Fihgame.Scripts.Model;

public class Item
{
    public string Name { get; set; }
    public string SpritePath { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public Rarity Rarity { get; set; }
    public int SellPrice { get; set; }

    public Item(string name, string spritePath, string description, 
        int quantity = 1, Rarity rarity = Rarity.Common, int sellPrice = 0)
    {
        Name = name;
        SpritePath = spritePath;
        Description = description;
        Quantity = quantity;
        Rarity = rarity;
        SellPrice = sellPrice;
    }

    public string GetRarityText() => Rarity switch
    {
        Rarity.Common    => "Common",
        Rarity.Uncommon  => "Uncommon",
        Rarity.Rare      => "Rare",
        Rarity.Legendary => "Legendary",
        _                => "Unknown"
    };

    public Color GetRarityColor() => Rarity switch
    {
        Rarity.Common    => new Color(1f, 1f, 1f),
        Rarity.Uncommon  => new Color(0.1f, 0.8f, 0.1f),
        Rarity.Rare      => new Color(0.1f, 0.4f, 1f),
        Rarity.Legendary => new Color(1f, 0.5f, 0f),
        _                => new Color(1f, 1f, 1f)
    };
}