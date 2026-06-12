using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.World.NPC;

public partial class ShopNPC : Area2D
{
    private ShopPanel _shopPanel;
    private bool _playerNearby = false;
    private Label _hintLabel;

    public void Initialize(ShopPanel shopPanel)
    {
        _shopPanel = shopPanel;
        _hintLabel = GetNode<Label>("HintLabel");
        _hintLabel.Visible = false;

        BodyEntered += OnBodyEntered;
        BodyExited  += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (_playerNearby && Input.IsActionJustPressed("interact"))
        {
            if (_shopPanel.Visible)
                _shopPanel.Visible = false;
            else
                _shopPanel.ShowShop();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player.Player)
        {
            _playerNearby = true;
            _hintLabel.Visible = true;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Player.Player)
        {
            _playerNearby = false;
            _hintLabel.Visible = false;
            _shopPanel.Visible = false;
        }
    }
}