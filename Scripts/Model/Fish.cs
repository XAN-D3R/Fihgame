namespace Fihgame.Scripts.Model;

public class Fish : CatchableEntity
{
    public Rarity Rarity { get; set; }
    public int SellPrice { get; set; }

    public Fish(string name, string spritePath, string description,
        Rarity rarity = Rarity.Common, int sellPrice = 0, float weight = 1f)
        : base(name, spritePath, description, weight)
    {
        Rarity = rarity;
        SellPrice = sellPrice;
    }
}