using Godot;

namespace Fihgame.Scripts.Model;

public class Fish
{
    public string Name { get; set; }
    public FishRarity Rarity { get; set; }
    public string SpritePath { get; set; }
    public string Description { get; set; }
    
    public Fish(string name, FishRarity rarity, string spritePath, string description)
    {
        Name = name;
        Rarity = rarity;
        SpritePath = spritePath;
        Description = description;
    }
    
    public string GetRarityText()
    {
        return Rarity switch
        {
            FishRarity.Common => "Common",
            FishRarity.Uncommon => "Uncommon",
            FishRarity.Rare => "Rare",
            FishRarity.Legendary => "Legendary",
            _ => "Unknown"
        };
    }
    
    public Color GetRarityColor()
    {
        return Rarity switch
        {
            FishRarity.Common => new Godot.Color(1f, 1f, 1f),        // Wit
            FishRarity.Uncommon => new Godot.Color(0.1f, 0.8f, 0.1f), // Groen
            FishRarity.Rare => new Godot.Color(0.1f, 0.4f, 1f),   // Blauw
            FishRarity.Legendary => new Godot.Color(1f, 0.5f, 0f), // Oranje
            _ => new Godot.Color(1f, 1f, 1f)
        };
    }
}