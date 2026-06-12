using System.Collections.Generic;
using Fihgame.Scripts.Model;
using Fihgame.Scripts.Player.Managers;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class ShopPanel : Panel
{
    private GridContainer _gridContainer;
    private Label _coinsLabel;
    private Label _feedbackLabel;
    private Timer _feedbackTimer;

    private Fihgame.Scripts.Player.Player _player;
    private ShopManager _shopManager;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("GridContainer");
        _coinsLabel = GetNode<Label>("CoinsLabel");
        _feedbackLabel = GetNode<Label>("FeedbackLabel");
        _feedbackTimer = GetNode<Timer>("FeedbackTimer");

        _feedbackLabel.Visible = false;
        _feedbackTimer.Timeout += () => _feedbackLabel.Visible = false;
    }

    public void Initialize(Fihgame.Scripts.Player.Player player)
    {
        _player = player;
        _shopManager = new ShopManager(player);
        _player.Inventory.OnChanged += Refresh;
    }

    public void ShowShop()
    {
        Visible = true;
        Refresh();
    }

    private void Refresh()
    {
        if (!Visible) return;

        foreach (Node child in _gridContainer.GetChildren())
            child.QueueFree();

        _coinsLabel.Text = $"Coins: {_player.Coins}";

        foreach (var entry in _player.Inventory.GetCount())
        {
            Item item = _player.Inventory.GetData(entry.Key);
            int count = entry.Value;
            _gridContainer.AddChild(CreateShopRow(item, count));
        }
    }

    private Control CreateShopRow(Item item, int count)
    {
        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(0, 50);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 8);

        var icon = new TextureRect();
        icon.Texture = GD.Load<Texture2D>(item.SpritePath);
        icon.CustomMinimumSize = new Vector2(36, 36);
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        icon.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var nameLabel = new Label();
        nameLabel.Text = $"{item.Name} x{count}";
        nameLabel.AddThemeColorOverride("font_color", item.GetRarityColor());
        nameLabel.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        nameLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var priceLabel = new Label();
        priceLabel.Text = $"{item.SellPrice * count}g";
        priceLabel.AddThemeColorOverride("font_color", new Color(1f, 0.85f, 0.2f)); // goud
        priceLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

        var sellOneBtn = new Button();
        sellOneBtn.Text = "Sell 1";
        sellOneBtn.CustomMinimumSize = new Vector2(60, 30);
        sellOneBtn.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        sellOneBtn.Pressed += () =>
        {
            int earned = _shopManager.SellItem(item.Name, sellAll: false);
            ShowFeedback($"+{earned} coins!");
        };

        var sellAllBtn = new Button();
        sellAllBtn.Text = "Sell All";
        sellAllBtn.CustomMinimumSize = new Vector2(60, 30);
        sellAllBtn.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        sellAllBtn.Pressed += () =>
        {
            int earned = _shopManager.SellItem(item.Name, sellAll: true);
            ShowFeedback($"+{earned} coins!");
        };

        hbox.AddChild(icon);
        hbox.AddChild(nameLabel);
        hbox.AddChild(priceLabel);
        hbox.AddChild(sellOneBtn);
        hbox.AddChild(sellAllBtn);
        panel.AddChild(hbox);
        return panel;
    }

    private void ShowFeedback(string message)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.Visible = true;
        _feedbackTimer.Start(2.0);
        _coinsLabel.Text = $"Coins: {_player.Coins}";
    }
}