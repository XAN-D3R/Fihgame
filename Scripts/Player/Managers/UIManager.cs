using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.Player.Managers;

public partial class UIManager : Node
{
    private Player _player;
    private InventoryPanel _inventoryPanel;
    private EquipmentPanel _equipmentPanel;
    
    private DeathMenu _deathMenu;
    private ShopPanel _shopPanel;

    public void Initialize(Player player, InventoryPanel inventoryPanel, DeathMenu deathMenu, ShopPanel shopPanel, EquipmentPanel equipmentPanel)
    {
        _player = player;
        _inventoryPanel = inventoryPanel;
        _deathMenu = deathMenu;
        _equipmentPanel = equipmentPanel;

        _inventoryPanel.SetInventory(_player.Inventory);
        _inventoryPanel.SetEquipment(_player.Equipment);
        _equipmentPanel.Initialize(_player);
        _player.OnInventoryPressed += HandleInventoryPressed;
    }

    private void HandleInventoryPressed()
    {
        bool show = !_inventoryPanel.Visible;
        _inventoryPanel.Visible = show;
        if (show)
            _inventoryPanel.ShowInventory(_player.Inventory);

        _equipmentPanel.Visible = show;
        if (show)
            _equipmentPanel.Refresh();
    }

    public void ShowDeathMenu()
    {
        _deathMenu.ShowDeathMenu(_player.Stats);
    }
    
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (_inventoryPanel.Visible)
                _inventoryPanel.Visible = false;
            else if (_shopPanel.Visible)
                _shopPanel.Visible = false;
        }
    }
}