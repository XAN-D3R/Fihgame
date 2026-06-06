using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.World;

public partial class DroppedItemEntity : Area2D
{
    private Sprite2D _sprite;
    private Label _label;
    private DroppedItem _item;

    public void Initialize(DroppedItem item)
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _label = GetNode<Label>("Label");
        _item = item;

        _sprite.Texture = GD.Load<Texture2D>(item.SpritePath);
        _label.Text = $"{item.Name} x{item.Quantity}";

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player.Player player)
        {
            player.Inventory.AddItem(_item);
            GD.Print($"Picked up {_item.Quantity}x {_item.Name}!");
            QueueFree();
        }
    }
}