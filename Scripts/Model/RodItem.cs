namespace Fihgame.Scripts.Model;

public class RodItem : Item
{
    public RodItem(string name, string spritePath, string description, int sellPrice = 0)
        : base(name, spritePath, description, quantity: 1, sellPrice: sellPrice) { }
}