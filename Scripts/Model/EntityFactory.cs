using Godot;

namespace Fihgame.Scripts.Model;

public static class EntityFactory
{
    private static readonly CatchableEntity[] _possibleEntities = {
        new Fish("Carp",  "res://Assets/Sprites/Fishing/Fish/Carp.png",          "A common carp. Often found in calm, shallow waters.", Rarity.Common),
        new Fish("Perch",        "res://Assets/Sprites/Fishing/Fish/Perch.png",   "A perch. Recognizable by its distinctive stripes.",   Rarity.Common),
        new Fish("Pike",      "res://Assets/Sprites/Fishing/Fish/Pike.png",   "A pike. A fierce predator lurking beneath the surface.",   Rarity.Uncommon),
        new Fish("Trout",      "res://Assets/Sprites/Fishing/Fish/Trout.png",   "A trout. Found only in clear, fast-flowing streams.",   Rarity.Rare),
        new Fish("Salmon",  "res://Assets/Sprites/Fishing/Fish/Salmon.png",    "A salmon. Extraordinarily rare and highly prized by fishermen.",    Rarity.Legendary),
        new SeaCreature("Bonerfish", "res://Assets/Sprites/Fishing/SeaCreature/Bonerfish.png", "A mysterious creature from the deep. Prepare to fight!", 100, 15, 2f,
            drops: new DroppedItem[]
            {
                new DroppedItem("Bone",  "res://Assets/Sprites/Items/Bone.png",  "A bone dropped by the Bonerfish.", 1),
                new DroppedItem("Scale", "res://Assets/Sprites/Items/Scale.png", "A scale dropped by the Bonerfish.", 2)
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
            Fish f         => new Fish(f.Name, f.SpritePath, f.Description, f.Rarity),
            _              => throw new System.Exception($"Unknown entity type: {template.GetType()}")
        };
    }
}