using System.Collections.Generic;
using Fihgame.Scripts.Model;
using Godot;
using Godot.Collections;

namespace Fihgame.Scripts.Data;

public static class DataLoader
{
    public static List<Fish> LoadFish()
    {
        var list = new List<Fish>();
        var json = LoadJson("res://Data/fish.json");

        foreach (var item in json)
        {
            var d = item.AsGodotDictionary();
            list.Add(new Fish(
                d["name"].AsString(),
                d["spritePath"].AsString(),
                d["description"].AsString(),
                ParseRarity(d["rarity"].AsString()),
                d["sellPrice"].AsInt32(),
                d["weight"].AsSingle(),
                d["minRodLevel"].AsInt32()
            ));
        }
        return list;
    }

    public static List<SeaCreature> LoadSeaCreatures()
    {
        var list = new List<SeaCreature>();
        var json = LoadJson("res://Data/sea_creatures.json");

        foreach (var item in json)
        {
            var d = item.AsGodotDictionary();
            var drops = new List<DroppedItem>();

            foreach (var drop in d["drops"].AsGodotArray())
            {
                var dd = drop.AsGodotDictionary();
                drops.Add(new DroppedItem(
                    dd["name"].AsString(),
                    dd["spritePath"].AsString(),
                    dd["description"].AsString(),
                    dd["quantity"].AsInt32(),
                    sellPrice: dd["sellPrice"].AsInt32()
                ));
            }

            list.Add(new SeaCreature(
                d["name"].AsString(),
                d["spritePath"].AsString(),
                d["description"].AsString(),
                d["maxHp"].AsInt32(),
                d["damage"].AsInt32(),
                d["attackSpeed"].AsSingle(),
                drops.ToArray(),
                d["weight"].AsSingle(),
                d["minRodLevel"].AsInt32()
            ));
        }
        return list;
    }

    public static List<RodItem> LoadRods()
    {
        var list = new List<RodItem>();
        var json = LoadJson("res://Data/rods.json");

        foreach (var item in json)
        {
            var d = item.AsGodotDictionary();
            list.Add(new RodItem(
                d["name"].AsString(),
                d["spritePath"].AsString(),
                d["description"].AsString(),
                d["sellPrice"].AsInt32(),
                d["fishingSpeed"].AsSingle(),
                d["seaCreatureChance"].AsSingle(),
                d["rodLevel"].AsInt32(),
                d["tracking"].AsSingle()
            ));
        }
        return list;
    }
    
    public static List<CraftingRecipe> LoadCraftingRecipes()
    {
        var rods = LoadRods();
        var list = new List<CraftingRecipe>();
        var json = LoadJson("res://Data/crafting_recipes.json");

        foreach (var item in json)
        {
            var d = item.AsGodotDictionary();
            var rodName = d["resultRod"].AsString();
            var rod = rods.Find(r => r.Name == rodName);

            if (rod == null)
            {
                GD.PrintErr($"[DataLoader] Rod not found: {rodName}");
                continue;
            }

            var ingredients = new List<(string, int)>();
            foreach (var ing in d["ingredients"].AsGodotArray())
            {
                var id = ing.AsGodotDictionary();
                ingredients.Add((id["itemName"].AsString(), id["quantity"].AsInt32()));
            }

            list.Add(new CraftingRecipe(rod, ingredients.ToArray()));
        }
        return list;
    }
    
    public static List<Item> LoadStartingInventory()
    {
        var fish = LoadFish();
        var creatures = LoadSeaCreatures();
        var rods = LoadRods();
        var list = new List<Item>();
        var json = LoadJson("res://Data/starting_inventory.json");

        foreach (var item in json)
        {
            var d = item.AsGodotDictionary();
            var type = d["type"].AsString();
            var name = d["name"].AsString();
            var quantity = d["quantity"].AsInt32();

            switch (type)
            {
                case "Fish":
                    var f = fish.Find(x => x.Name == name);
                    if (f != null) for (int i = 0; i < quantity; i++)
                        list.Add(new FishItem(f.Name, f.SpritePath, f.Description, f.Rarity, f.SellPrice));
                    break;
                case "DroppedItem":
                    foreach (var sc in creatures)
                    foreach (var drop in sc.Drops)
                        if (drop.Name == name)
                            list.Add(new DroppedItem(drop.Name, drop.SpritePath, drop.Description, quantity, drop.Rarity, drop.SellPrice));
                    break;
                case "Rod":
                    var rod = rods.Find(x => x.Name == name);
                    if (rod != null) list.Add(rod);
                    break;
                default:
                    GD.PrintErr($"[DataLoader] Unknown type: {type}");
                    break;
            }
        }
        return list;
    }

    private static Array LoadJson(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var json = new Json();
        json.Parse(file.GetAsText());
        return json.Data.AsGodotArray();
    }

    private static Rarity ParseRarity(string rarity) => rarity switch
    {
        "Common"    => Rarity.Common,
        "Uncommon"  => Rarity.Uncommon,
        "Rare"      => Rarity.Rare,
        "Legendary" => Rarity.Legendary,
        _           => Rarity.Common
    };
}