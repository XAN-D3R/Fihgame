using System.Collections.Generic;
using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class InventoryPanel : Panel
{
    private GridContainer _gridContainer;
    private Panel _detailPanel;
    private TextureRect _fishSprite;
    private Label _fishName;
    private Label _rarityLabel;
    private Label _description;
    private Label _countLabel;
    
    [Export] public Texture2D PlaceholderTexture;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("GridContainer");
        _detailPanel = GetNode<Panel>("DetailPanel");
        _fishSprite = GetNode<TextureRect>("DetailPanel/FishSprite");
        _fishName = GetNode<Label>("DetailPanel/FishName");
        _rarityLabel = GetNode<Label>("DetailPanel/RarityLabel");
        _description = GetNode<Label>("DetailPanel/Description");
        _countLabel = GetNode<Label>("DetailPanel/CountLabel");
    }

    public void ShowInventory(Inventory inventory)
    {
        // Verwijder oude items
        foreach (Node child in _gridContainer.GetChildren())
            child.QueueFree();

        // Voeg vis icoontjes toe
        foreach (var entry in inventory.GetFishCount())
        {
            Fish fish = inventory.GetFishData(entry.Key);
            int count = entry.Value;

            var button = new TextureButton();
            button.TextureNormal = GD.Load<Texture2D>(fish.SpritePath);
            button.CustomMinimumSize = new Vector2(48, 48);
            button.Pressed += () => ShowDetail(fish, count);
            _gridContainer.AddChild(button);
        }

        _detailPanel.Visible = false;
        Visible = true;
    }

    private void ShowDetail(Fish fish, int count)
    {
        _fishSprite.Texture = GD.Load<Texture2D>(fish.SpritePath);
        _fishName.Text = fish.Name;
        _rarityLabel.Text = fish.GetRarityText();
        _rarityLabel.AddThemeColorOverride("font_color", fish.GetRarityColor());
        _description.Text = fish.Description;
        _countLabel.Text = $"Amount: {count}";
        _detailPanel.Visible = true;
    }
}