namespace Fihgame.Scripts.Model;

public class Fish : CatchableEntity
{
    public Rarity Rarity { get; set; }

    public Fish(string name, string spritePath, string description, Rarity rarity = Rarity.Common)
        : base(name, spritePath, description)
    {
        Rarity = rarity;
    }
}