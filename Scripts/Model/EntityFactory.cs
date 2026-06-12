using Godot;

namespace Fihgame.Scripts.Model;

public static class EntityFactory
{
    private static readonly CatchableEntity[] _possibleEntities = {
        new Fish("Carp",   "res://Assets/Sprites/Fishing/Fish/Carp.png",   "A common carp.",          Rarity.Common,    sellPrice: 10, weight: 1f),
        new Fish("Perch",  "res://Assets/Sprites/Fishing/Fish/Perch.png",  "Recognizable stripes.",   Rarity.Common,    sellPrice: 10, weight: 2f),
        new Fish("Pike",   "res://Assets/Sprites/Fishing/Fish/Pike.png",   "A fierce predator.",      Rarity.Uncommon,  sellPrice: 25, weight: 5f),
        new Fish("Trout",  "res://Assets/Sprites/Fishing/Fish/Trout.png",  "Clear, fast streams.",    Rarity.Rare,      sellPrice: 60, weight: 7),
        new Fish("Salmon", "res://Assets/Sprites/Fishing/Fish/Salmon.png", "Extraordinarily rare.",   Rarity.Legendary, sellPrice: 150, weight: 10),
        new SeaCreature("Bonerfish", "res://Assets/Sprites/Fishing/SeaCreature/Bonerfish.png", "A mysterious creature from the deep. Prepare to fight!", 100, 15, 2f,
            drops: new DroppedItem[]
            {
                new DroppedItem("Bone",  "res://Assets/Sprites/Items/Bone.png",  "A bone dropped by the Bonerfish.",  1, sellPrice: 15),
                new DroppedItem("Scale", "res://Assets/Sprites/Items/Scale.png", "A scale dropped by the Bonerfish.", 2, sellPrice: 25)
            }, weight: 7)
    };

    public static CatchableEntity CreateRandom()
    {
        float totalWeight = 0f;
        foreach (var e in _possibleEntities)
            totalWeight += 1f / e.Weight;

        float roll = (float)GD.RandRange(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in _possibleEntities)
        {
            cumulative += 1f / e.Weight;
            if (roll <= cumulative)
                return Clone(e);
        }

        return Clone(_possibleEntities[0]);
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