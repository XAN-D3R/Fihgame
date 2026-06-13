namespace Fihgame.Scripts.Model;

public class CatchableEntity : Item
{
    public float Weight { get; set; }
    public int MinRodLevel { get; set; }

    public CatchableEntity(string name, string spritePath, string description,
        float weight = 1f, int minRodLevel = 1)
        : base(name, spritePath, description)
    {
        Weight = weight;
        MinRodLevel = minRodLevel;
    }
}