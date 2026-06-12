using Fihgame.Scripts.Model;

namespace Fihgame.Scripts.Player.Managers;

public class ShopManager
{
    private Player _player;

    public ShopManager(Player player)
    {
        _player = player;
    }

    public int SellItem(string itemName, bool sellAll = false)
    {
        var count = _player.Inventory.GetCount();
        if (!count.ContainsKey(itemName)) return 0;

        var item = _player.Inventory.GetData(itemName);
        int quantity = sellAll ? count[itemName] : 1;
        int earned = item.SellPrice * quantity;

        _player.Inventory.RemoveItem(itemName, quantity);
        _player.Coins += earned;

        return earned;
    }

    public int SellAll()
    {
        int total = 0;
        var keys = new System.Collections.Generic.List<string>(_player.Inventory.GetCount().Keys);
        foreach (var key in keys)
            total += SellItem(key, sellAll: true);
        return total;
    }
}