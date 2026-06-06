using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.Player.Managers;

public partial class UIManager : Node
{
    private Player _player;
    private InventoryPanel _inventoryPanel;
    private DeathMenu _deathMenu;

    public void Initialize(Player player, InventoryPanel inventoryPanel, DeathMenu deathMenu)
    {
        _player = player;
        _inventoryPanel = inventoryPanel;
        _deathMenu = deathMenu;

        _inventoryPanel.SetInventory(_player.Inventory);
        _player.OnInventoryPressed += HandleInventoryPressed;
    }

    private void HandleInventoryPressed()
    {
        if (_inventoryPanel.Visible)
            _inventoryPanel.Visible = false;
        else
            _inventoryPanel.ShowInventory(_player.Inventory);
    }

    public void ShowDeathMenu()
    {
        _deathMenu.ShowDeathMenu(_player.Stats);
    }
}