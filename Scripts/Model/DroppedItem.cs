namespace Fihgame.Scripts.Model;

public class DroppedItem : Item
{
    public DroppedItem(string name, string spritePath, string description = "",
        int quantity = 1, Rarity rarity = Rarity.Common, int sellPrice = 0)
        : base(name, spritePath, description, quantity, rarity, sellPrice) { }
}