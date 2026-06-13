using System.Collections.Generic;
using Fihgame.Scripts.Data;
using Godot;

namespace Fihgame.Scripts.Model;

public static class EntityFactory
{
    private static CatchableEntity[] _possibleEntities;
    
    public static void Initialize()
    {
        var entities = new List<CatchableEntity>();
        entities.AddRange(DataLoader.LoadFish());
        entities.AddRange(DataLoader.LoadSeaCreatures());
        _possibleEntities = entities.ToArray();
    }

    public static CatchableEntity CreateRandom(RodItem rod)
    {
        var pool = System.Array.FindAll(_possibleEntities, e => e.MinRodLevel <= rod.RodLevel);

        float roll = (float)GD.RandRange(0f, 100f);
        if (roll < rod.SeaCreatureChance)
        {
            var creatures = System.Array.FindAll(pool, e => e is SeaCreature);
            if (creatures.Length > 0)
                return Clone(GetWeightedRandom(creatures, rod.Tracking));
        }

        var fish = System.Array.FindAll(pool, e => e is Fish);
        return Clone(GetWeightedRandom(fish, rod.Tracking));
    }

    private static CatchableEntity GetWeightedRandom(CatchableEntity[] pool, float tracking)
    {
        float totalWeight = 0f;
        foreach (var e in pool)
        {
            float w = 1f / e.Weight;
            if (e is Fish f && f.Rarity != Rarity.Common)
                w *= 1f + tracking / 100f;
            totalWeight += w;
        }

        float r = (float)GD.RandRange(0f, totalWeight);
        float cumulative = 0f;
        foreach (var e in pool)
        {
            float w = 1f / e.Weight;
            if (e is Fish f && f.Rarity != Rarity.Common)
                w *= 1f + tracking / 100f;
            cumulative += w;
            if (r <= cumulative)
                return Clone(e);
        }

        return Clone(pool[0]);
    }

    private static CatchableEntity Clone(CatchableEntity template)
    {
        return template switch
        {
            SeaCreature sc => new SeaCreature(sc.Name, sc.SpritePath, sc.Description, sc.MaxHp, sc.Damage, sc.AttackSpeed, sc.Drops),
            Fish f => new Fish(f.Name, f.SpritePath, f.Description, f.Rarity, f.SellPrice),
            _              => throw new System.Exception($"Unknown entity type: {template.GetType()}")
        };
    }
}