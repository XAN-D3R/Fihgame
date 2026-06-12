namespace Fihgame.Scripts.Model;

public static class CraftingRecipes
{
    public static readonly CraftingRecipe[] All =
    {
        new CraftingRecipe(
            new RodItem("Intermediate Rod", "res://Assets/Sprites/Fishing/Rod/intermediate_rod_48x48.png",
                "A rod actually used for fishing", sellPrice: 20),
            ("Bone", 2), ("Scale", 1)
        ),
        new CraftingRecipe(
            new RodItem("Sea Rod", "res://Assets/Sprites/Fishing/Rod/sea_rod_48x48.png",
                "The rod of the sea", sellPrice: 100),
            ("Bone", 10), ("Salmon", 2), ("Carp", 10)
        )
    };
}