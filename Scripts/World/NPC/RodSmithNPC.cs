using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.World.NPC;

public partial class RodSmithNPC : Area2D
{
    private CraftingPanel _craftingPanel;
    private bool _playerNearby = false;
    private Label _hintLabel;

    public void Initialize(CraftingPanel craftingPanel)
    {
        _craftingPanel = craftingPanel;
        _hintLabel = GetNode<Label>("HintLabel");
        _hintLabel.Visible = false;

        BodyEntered += OnBodyEntered;
        BodyExited  += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (_playerNearby && Input.IsActionJustPressed("interact"))
        {
            if (_craftingPanel.Visible)
                _craftingPanel.Visible = false;
            else
                _craftingPanel.ShowCrafting();
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
            _craftingPanel.Visible = false;
        }
    }
}