namespace Fihgame.Scripts.Model;

public class CraftingRecipe
{
    public string ResultName { get; }
    public RodItem Result { get; }
    public (string itemName, int quantity)[] Ingredients { get; }

    public CraftingRecipe(RodItem result, params (string, int)[] ingredients)
    {
        Result = result;
        Ingredients = ingredients;
    }

    public bool CanCraft(Inventory inventory)
    {
        foreach (var (itemName, quantity) in Ingredients)
        {
            if (inventory.GetQuantity(itemName) < quantity) return false;
        }
        return true;
    }
}