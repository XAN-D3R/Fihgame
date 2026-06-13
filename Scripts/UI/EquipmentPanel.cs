using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class EquipmentPanel : Panel
{
    private Fihgame.Scripts.Player.Player _player;

    private Panel _contentPanel;
    private Button _collapseButton;
    private TextureRect _rodIcon;
    private Label _rodName;
    private Label _rodStats;
    private Button _unequipBtn;
    private Panel _rodSlot;
    private bool _collapsed = false;

    public void Initialize(Fihgame.Scripts.Player.Player player)
    {
        _player = player;

        _contentPanel = GetNode<Panel>("VBoxContainer/ContentPanel");
        _collapseButton = GetNode<Button>("VBoxContainer/Header/CollapseButton");
        _rodIcon = GetNode<TextureRect>("VBoxContainer/ContentPanel/MarginContainer/ContentVBox/RodHBox/RodSlot/RodIcon");
        _rodName = GetNode<Label>("VBoxContainer/ContentPanel/MarginContainer/ContentVBox/RodHBox/RodInfoVBox/RodName");
        _rodStats = GetNode<Label>("VBoxContainer/ContentPanel/MarginContainer/ContentVBox/RodHBox/RodInfoVBox/RodStats");
        _unequipBtn = GetNode<Button>("VBoxContainer/ContentPanel/MarginContainer/ContentVBox/UnequipButton");
        _rodSlot = GetNode<Panel>("VBoxContainer/ContentPanel/MarginContainer/ContentVBox/RodHBox/RodSlot");

        _collapseButton.Pressed += ToggleCollapse;
        _unequipBtn.Pressed += OnUnequipPressed;

        _rodSlot.SetDragForwarding(
            Callable.From((Vector2 _) =>
            {
                var rod = _player.Equipment.EquippedRod;
                if (rod == Equipment.DefaultRod) return new Variant();

                var preview = new TextureRect();
                preview.Texture = GD.Load<Texture2D>(rod.SpritePath);
                preview.CustomMinimumSize = new Vector2(40, 40);
                _rodSlot.SetDragPreview(preview);
                return new RodDragData(rod);
            }),
            Callable.From((Vector2 _, Variant data) => data.Obj is RodDragData),
            Callable.From((Vector2 _, Variant data) =>
            {
                if (data.Obj is RodDragData dragData)
                {
                    var current = _player.Equipment.EquippedRod;
                    if (current != Equipment.DefaultRod)
                        _player.Inventory.AddItem(current);

                    _player.Inventory.RemoveItem(dragData.Rod.Name);
                    _player.Equipment.EquipRod(dragData.Rod);
                    Refresh();
                }
            })
        );

        Refresh();
    }

    private void OnUnequipPressed()
    {
        var current = _player.Equipment.EquippedRod;
        if (current != Equipment.DefaultRod)
            _player.Inventory.AddItem(current);

        _player.Equipment.UnequipRod();
        Refresh();
    }

    private void ToggleCollapse()
    {
        _collapsed = !_collapsed;
        _contentPanel.Visible = !_collapsed;
        _collapseButton.Text = _collapsed ? "+" : "−";
    }

    public void Refresh()
    {
        if (_player == null) return;
        var rod = _player.Equipment.EquippedRod;
        _rodIcon.Texture = GD.Load<Texture2D>(rod.SpritePath);
        _rodName.Text = rod.Name;
        _rodStats.Text = $"Fishing Speed: {rod.FishingSpeed}x\nSea Creature Chance: {rod.SeaCreatureChance}%\nRod Level: {rod.RodLevel}\nTracking: {rod.Tracking}";
        _unequipBtn.Visible = rod != Equipment.DefaultRod;
    }
}