using Fihgame.Scripts.Data;

namespace Fihgame.Scripts.Model;

public class Equipment
{
    private static RodItem _defaultRod;
    public static RodItem DefaultRod => _defaultRod ??= DataLoader.LoadRods().Find(r => r.Name == "Basic Rod");

    public RodItem EquippedRod { get; private set; } = DefaultRod;

    public void EquipRod(RodItem rod) => EquippedRod = rod;
    public void UnequipRod() => EquippedRod = DefaultRod;
}