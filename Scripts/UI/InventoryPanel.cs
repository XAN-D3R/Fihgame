using System;
using System.Collections.Generic;
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

    private Inventory _inventory;

    [Export] public Texture2D PlaceholderTexture;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("GridContainer");
        _detailPanel = GetNode<Panel>("DetailPanel");
        _itemSprite = GetNode<TextureRect>("DetailPanel/FishSprite");
        _itemName = GetNode<Label>("DetailPanel/FishName");
        _rarityLabel = GetNode<Label>("DetailPanel/RarityLabel");
        _description = GetNode<Label>("DetailPanel/Description");
        _countLabel = GetNode<Label>("DetailPanel/CountLabel");
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
        _inventory.OnChanged += Refresh;
    }

    private void Refresh()
    {
        if (Visible)
            ShowInventory(_inventory);
    }

    public void ShowInventory(Inventory inventory)
    {
        foreach (Node child in _gridContainer.GetChildren())
            child.QueueFree();

        foreach (var entry in inventory.GetCount())
        {
            Item item = inventory.GetData(entry.Key);
            int count = entry.Value;
            _gridContainer.AddChild(CreateSlot(item.SpritePath, count, () => ShowDetail(item, count)));
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
        _rarityLabel.Visible = true;
        _rarityLabel.Text = item.GetRarityText();
        _rarityLabel.AddThemeColorOverride("font_color", item.GetRarityColor());
        _detailPanel.Visible = true;
    }

    private Control CreateSlot(string spritePath, int count, Action onPressed)
    {
        var container = new Control();
        container.CustomMinimumSize = new Vector2(48, 48);

        var button = new TextureButton();
        button.TextureNormal = GD.Load<Texture2D>(spritePath);
        button.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;
        button.SetAnchorsPreset(LayoutPreset.FullRect);
        button.Pressed += onPressed;

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