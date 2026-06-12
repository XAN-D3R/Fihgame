namespace Fihgame.Scripts.Model;

public class FishItem : Item
{
    public FishItem(string name, string spritePath, string description,
        Rarity rarity = Rarity.Common, int sellPrice = 0)
        : base(name, spritePath, description, 1, rarity, sellPrice) { }
}