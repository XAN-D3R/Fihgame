using Godot;

namespace Fihgame.Scripts.Model;

public static class EntityFactory
{
    private static readonly CatchableEntity[] _possibleEntities = {
        new Fish("Carp",   "res://Assets/Sprites/Fishing/Fish/Carp.png",   "A common carp.",          Rarity.Common,    sellPrice: 10),
        new Fish("Perch",  "res://Assets/Sprites/Fishing/Fish/Perch.png",  "Recognizable stripes.",   Rarity.Common,    sellPrice: 10),
        new Fish("Pike",   "res://Assets/Sprites/Fishing/Fish/Pike.png",   "A fierce predator.",      Rarity.Uncommon,  sellPrice: 25),
        new Fish("Trout",  "res://Assets/Sprites/Fishing/Fish/Trout.png",  "Clear, fast streams.",    Rarity.Rare,      sellPrice: 60),
        new Fish("Salmon", "res://Assets/Sprites/Fishing/Fish/Salmon.png", "Extraordinarily rare.",   Rarity.Legendary, sellPrice: 150),
        new SeaCreature("Bonerfish", "res://Assets/Sprites/Fishing/SeaCreature/Bonerfish.png", "A mysterious creature from the deep. Prepare to fight!", 100, 15, 2f,
            drops: new DroppedItem[]
            {
                new DroppedItem("Bone",  "res://Assets/Sprites/Items/Bone.png",  "A bone dropped by the Bonerfish.",  1, sellPrice: 15),
                new DroppedItem("Scale", "res://Assets/Sprites/Items/Scale.png", "A scale dropped by the Bonerfish.", 2, sellPrice: 25)
            }),
    };

    public static CatchableEntity CreateRandom()
    {
        var template = _possibleEntities[GD.RandRange(0, _possibleEntities.Length - 1)];
        return Clone(template);
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