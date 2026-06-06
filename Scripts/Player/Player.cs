using Fihgame.Scripts.Model;
using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.Player;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 150f;
    [Export] public float FishingWaitTime = 2.5f; // Wacht tijd in seconden

    private AnimatedSprite2D _sprite;
    private Sprite2D _fishingSprite;
    
    private FishCaughtPopup _fishCaughtPopup;
    private Inventory _inventory = new Inventory();
    private InventoryPanel _inventoryPanel;
    
    private TileMapLayer _waterLayer;

    private string _lastDirection = "down";
    
    // Fishing variabelen
    private bool _isFishing = false;
    private float _fishingTimer = 0f;
    
    // Lijst van mogelijke vissen
    private Fish[] _possibleFish = {
        new Fish("Carp",  FishRarity.Common,        "res://Assets/Sprites/Fishing/Fish/Carp.png", "A common carp. Often found in calm, shallow waters."),
        new Fish("Perch",   FishRarity.Common,        "res://Assets/Sprites/Fishing/Fish/Perch.png",   "A perch. Recognizable by its distinctive stripes."),
        new Fish("Pike",   FishRarity.Uncommon,      "res://Assets/Sprites/Fishing/Fish/Pike.png",   "A pike. A fierce predator lurking beneath the surface."),
        new Fish("Trout",   FishRarity.Rare,      "res://Assets/Sprites/Fishing/Fish/Trout.png",   "A trout. Found only in clear, fast-flowing streams."),
        new Fish("Salmon",    FishRarity.Legendary,  "res://Assets/Sprites/Fishing/Fish/Salmon.png",    "A salmon. Extraordinarily rare and highly prized by fishermen."),
    };

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _fishingSprite = GetNode<Sprite2D>("FishingSprite");
        _waterLayer = GetNode<TileMapLayer>("../WaterLayer");
        _fishCaughtPopup = GetNode<FishCaughtPopup>("../CanvasLayer/FishCaughtPopup");
        _inventoryPanel = GetNode<InventoryPanel>("../CanvasLayer/InventoryPanel");
        var camera = GetNode<Camera2D>("Camera2D");
        camera.MakeCurrent();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Niet bewegen tijdens het vissen
        if (_isFishing)
        {
            _fishingTimer -= (float)delta;
            if (_fishingTimer <= 0f)
            {
                CatchFish();
            }
            return;
        }

        // Vissen starten met F
        if (Input.IsActionJustPressed("fish"))
        {
            if (IsFacingWater())
            {
                StartFishing();
            }
            else
            {
                GD.Print("Je staat niet naast water!");
            }
            return;
        }
        
        if (Input.IsActionJustPressed("inventory"))
        {
            GD.Print("Inventory activated");

            if (_inventoryPanel.Visible)
                _inventoryPanel.Visible = false;
            else
                _inventoryPanel.ShowInventory(_inventory);
            return;
        }

        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("ui_right")) direction.X += 1;
        if (Input.IsActionPressed("ui_left"))  direction.X -= 1;
        if (Input.IsActionPressed("ui_down"))  direction.Y += 1;
        if (Input.IsActionPressed("ui_up"))    direction.Y -= 1;

        Velocity = direction.Normalized() * Speed;
        MoveAndSlide();

        if (direction == Vector2.Zero)
        {
            _sprite.Play($"idle_{_lastDirection}");
            _sprite.Stop();
        }
        else if (direction.X > 0)
        {
            _lastDirection = "right";
            _sprite.Play("walk_right");
        }
        else if (direction.X < 0)
        {
            _lastDirection = "left";
            _sprite.Play("walk_left");
        }
        else if (direction.Y > 0)
        {
            _lastDirection = "down";
            _sprite.Play("walk_down");
        }
        else
        {
            _lastDirection = "up";
            _sprite.Play("walk_up");
        }
    }

    private void StartFishing()
    {
        _sprite.Play($"idle_{_lastDirection}");
        _sprite.Stop();
        _isFishing = true;
        _fishingTimer = FishingWaitTime;
        _fishingSprite.Visible = true;
        UpdateFishingRodPosition();
        GD.Print("Hengel uitgegooid... wachten op een vis!");
    }

    private void CatchFish()
    {
        _isFishing = false;
        _fishingSprite.Visible = false;
    
        Fish caughtFish = _possibleFish[GD.RandRange(0, _possibleFish.Length - 1)];
        _inventory.AddFish(caughtFish);
        _fishCaughtPopup.ShowFish(caughtFish);
        
        foreach (var entry in _inventory.GetFishCount())
        {
            GD.Print($"{entry.Key} x{entry.Value}");
        }
    }
    
    private void UpdateFishingRodPosition()
    {
        switch (_lastDirection)
        {
            case "right":
                _fishingSprite.Position = new Vector2(8, 0);
                _fishingSprite.FlipH = false;
                _fishingSprite.FlipV = false;
                break;
            case "left":
                _fishingSprite.Position = new Vector2(-8, 0);
                _fishingSprite.FlipH = true;
                _fishingSprite.FlipV = false;
                break;
            case "down":
                _fishingSprite.Position = new Vector2(0, 8);
                _fishingSprite.FlipH = false;
                _fishingSprite.FlipV = false;
                break;
            case "up":
                _fishingSprite.Position = new Vector2(0, -8);
                _fishingSprite.FlipH = false;
                _fishingSprite.FlipV = false;
                break;
        }
        _fishingSprite.Rotation = 0f;
    }
    
    private bool IsFacingWater()
    {
        Vector2 checkPosition = GlobalPosition;
        
        const int distance = 32;
    
        switch (_lastDirection)
        {
            case "right": checkPosition.X += distance; break;
            case "left":  checkPosition.X -= distance; break;
            case "down":  checkPosition.Y += distance; break;
            case "up":    checkPosition.Y -= distance; break;
        }
    
        Vector2I tilePos = _waterLayer.LocalToMap(_waterLayer.ToLocal(checkPosition));
    
        return _waterLayer.GetCellSourceId(tilePos) != -1;
    }
}