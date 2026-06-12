using System.Collections.Generic;
using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class CraftingPanel : Panel
{
    private GridContainer _gridContainer;
    private Label _feedbackLabel;
    private Timer _feedbackTimer;
    
    private Fihgame.Scripts.Player.Player _player;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("GridContainer");
        _feedbackLabel = GetNode<Label>("FeedbackLabel");
        _feedbackTimer = GetNode<Timer>("FeedbackTimer");
        
        _feedbackLabel.Visible = false;
        _feedbackTimer.Timeout += () => _feedbackLabel.Visible = false;
    }
    
    public void Initialize(Fihgame.Scripts.Player.Player player)
    {
        _player = player;
        _player.Inventory.OnChanged += Refresh;
    }
    
    public void ShowCrafting()
    {
        Visible = true;
        Refresh();
    }
    
    private void Refresh()
    {
        if (!Visible) return;

        foreach (Node child in _gridContainer.GetChildren())
            child.QueueFree();

        foreach (var recipe in CraftingRecipes.All)
        {
            _gridContainer.AddChild(CreateRecipeRow(recipe));
        }
    }
    
    private Control CreateRecipeRow(CraftingRecipe recipe)
    {
        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(0, 50);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 8);

        var icon = new TextureRect();
        icon.Texture = GD.Load<Texture2D>(recipe.Result.SpritePath);
        icon.CustomMinimumSize = new Vector2(36, 36);
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        icon.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var nameLabel = new Label();
        nameLabel.Text = recipe.Result.Name;
        nameLabel.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        nameLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var ingredientsLabel = new Label();
        var parts = new List<string>();
        foreach (var (itemName, quantity) in recipe.Ingredients)
        {
            int has = _player.Inventory.GetQuantity(itemName);
            parts.Add($"{itemName} {has}/{quantity}");
        }
        ingredientsLabel.Text = string.Join(", ", parts);
        ingredientsLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var craftBtn = new Button();
        craftBtn.Text = "Craft";
        craftBtn.CustomMinimumSize = new Vector2(60, 30);
        craftBtn.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        craftBtn.Disabled = !recipe.CanCraft(_player.Inventory);
        craftBtn.Pressed += () =>
        {
            if (!recipe.CanCraft(_player.Inventory)) return;

            foreach (var (itemName, quantity) in recipe.Ingredients)
                _player.Inventory.RemoveItem(itemName, quantity);

            _player.Inventory.AddItem(recipe.Result);
            ShowFeedback($"Crafted {recipe.Result.Name}!");
        };

        hbox.AddChild(icon);
        hbox.AddChild(nameLabel);
        hbox.AddChild(ingredientsLabel);
        hbox.AddChild(craftBtn);
        panel.AddChild(hbox);
        return panel;
    }

    private void ShowFeedback(string message)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.Visible = true;
        _feedbackTimer.Start(2.0);
    }
}