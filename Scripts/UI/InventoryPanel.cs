using System;
using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class InventoryPanel : Panel
{
    private GridContainer _gridContainer;
    private Panel _detailPanel;
    private TextureRect _itemSprite;
    private Label _itemName;
    private Label _rarityLabel;
    private Label _description;
    private Label _countLabel;
    private Button _equipButton;

    private Inventory _inventory;
    private Equipment _equipment;
    private EquipmentPanel _equipmentPanel;
    private Player.Player _player;

    private Action _currentEquipAction;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/GridContainer");
        _detailPanel = GetNode<Panel>("DetailPanel");
        _itemSprite = GetNode<TextureRect>("DetailPanel/FishSprite");
        _itemName = GetNode<Label>("DetailPanel/FishName");
        _rarityLabel = GetNode<Label>("DetailPanel/RarityLabel");
        _description = GetNode<Label>("DetailPanel/Description");
        _countLabel = GetNode<Label>("DetailPanel/CountLabel");
        _equipButton = GetNode<Button>("DetailPanel/EquipButton");
        _equipButton.Visible = false;

        SetDragForwarding(
            Callable.From((Vector2 _) => new Variant()),
            Callable.From((Vector2 _, Variant data) => data.Obj is RodDragData),
            Callable.From((Vector2 _, Variant data) =>
            {
                if (data.Obj is RodDragData dragData)
                {
                    if (_player.Equipment.EquippedRod == dragData.Rod)
                    {
                        _player.Equipment.UnequipRod();
                        _inventory.AddItem(dragData.Rod);
                        _equipmentPanel?.Refresh();
                    }
                }
            })
        );
    }

    public void SetPlayer(Player.Player player) => _player = player;
    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
        _inventory.OnChanged += Refresh;
    }
    public void SetEquipment(Equipment equipment) => _equipment = equipment;
    public void SetEquipmentPanel(EquipmentPanel panel) => _equipmentPanel = panel;

    private void Refresh()
    {
        if (Visible) ShowInventory(_inventory);
    }

    public void ShowInventory(Inventory inventory)
    {
        foreach (Node child in _gridContainer.GetChildren())
            child.QueueFree();

        foreach (var entry in inventory.GetCount())
        {
            Item item = inventory.GetData(entry.Key);
            int count = entry.Value;
            _gridContainer.AddChild(CreateSlot(item, count));
        }

        _detailPanel.Visible = false;
        Visible = true;
    }

    private void ShowDetail(Item item, int count)
    {
        _itemSprite.Texture = GD.Load<Texture2D>(item.SpritePath);
        _itemName.Text = item.Name;
        _description.Text = item.Description;
        _countLabel.Text = $"Amount: {count}";
        _rarityLabel.Text = item.GetRarityText();
        _rarityLabel.AddThemeColorOverride("font_color", item.GetRarityColor());
        _detailPanel.Visible = true;

        if (_currentEquipAction != null)
        {
            _equipButton.Pressed -= _currentEquipAction;
            _currentEquipAction = null;
        }

        _equipButton.Visible = item is RodItem;

        if (item is RodItem rod)
        {
            _currentEquipAction = () =>
            {
                var current = _equipment.EquippedRod;
                if (current != Equipment.DefaultRod)
                    _inventory.AddItem(current);

                _inventory.RemoveItem(rod.Name);
                _equipment.EquipRod(rod);
                _equipmentPanel?.Refresh();
                _detailPanel.Visible = false;
            };
            _equipButton.Pressed += _currentEquipAction;
        }
    }

    private Control CreateSlot(Item item, int count)
    {
        var container = new Control();
        container.CustomMinimumSize = new Vector2(48, 48);

        var button = new TextureButton();
        button.TextureNormal = GD.Load<Texture2D>(item.SpritePath);
        button.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;
        button.SetAnchorsPreset(LayoutPreset.FullRect);
        button.Pressed += () => ShowDetail(item, count);

        if (item is RodItem rod)
        {
            button.SetDragForwarding(
                Callable.From((Vector2 _) =>
                {
                    var preview = new TextureRect();
                    preview.Texture = GD.Load<Texture2D>(rod.SpritePath);
                    preview.CustomMinimumSize = new Vector2(40, 40);
                    preview.ZIndex = 100;
                    button.SetDragPreview(preview);
                    return new RodDragData(rod);
                }),
                Callable.From((Vector2 _, Variant __) => false),
                Callable.From((Vector2 _, Variant __) => { })
            );
        }

        var label = new Label();
        label.Text = count > 1 ? $"x{count}" : "";
        label.SetAnchorsPreset(LayoutPreset.FullRect);
        label.HorizontalAlignment = HorizontalAlignment.Right;
        label.VerticalAlignment = VerticalAlignment.Bottom;
        label.AddThemeFontSizeOverride("font_size", 10);
        label.AddThemeColorOverride("font_color", Colors.White);

        container.AddChild(button);
        container.AddChild(label);
        return container;
    }
}