namespace Fihgame.Scripts.Model;

public class CatchableEntity : Item
{
    public float Weight { get; set; }

    public CatchableEntity(string name, string spritePath, string description, float weight = 1f)
        : base(name, spritePath, description)
    {
        Weight = weight;
    }
}