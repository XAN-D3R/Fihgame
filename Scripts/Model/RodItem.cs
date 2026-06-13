namespace Fihgame.Scripts.Model;

public class RodItem : Item
{
    public float FishingSpeed { get; }       // 1.0 = default, 2.0 = 2x faster
    public float SeaCreatureChance { get; }  // 0-100%
    public int RodLevel { get; }             // 1-5
    public float Tracking { get; }           // 0+, 100 = double chance on rare entities

    public RodItem(string name, string spritePath, string description,
        int sellPrice = 0,
        float fishingSpeed = 1f,
        float seaCreatureChance = 10f,
        int rodLevel = 1,
        float tracking = 0f)
        : base(name, spritePath, description, quantity: 1, sellPrice: sellPrice)
    {
        FishingSpeed = fishingSpeed;
        SeaCreatureChance = seaCreatureChance;
        RodLevel = rodLevel;
        Tracking = tracking;
    }
}