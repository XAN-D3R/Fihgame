using Fihgame.Scripts.Player.Managers;
using Fihgame.Scripts.UI;
using Fihgame.Scripts.World.NPC;
using Godot;


namespace Fihgame.Scripts.World;

public partial class GameManager : Node
{
    private Scripts.Player.Player _player;
    private FishingManager _fishingManager;
    private CombatManager _combatManager;
    private UIManager _uiManager;
    private ShopNPC _shopNpc;

    public override void _Ready()
    {
        _player = GetNode<Scripts.Player.Player>("Entities/Player");

        var waterLayer = GetNode<TileMapLayer>("WaterLayer");
        var fishingSprite = _player.GetNode<Sprite2D>("FishingSprite");
        var attackArea = _player.GetNode<Area2D>("AttackArea");
        var seaCreatureScene = GD.Load<PackedScene>("res://Scenes/Entities/SeaCreatureEntity.tscn");

        var fishCaughtPopup = GetNode<FishCaughtPopup>("CanvasLayer/FishCaughtPopup");
        var inventoryPanel = GetNode<InventoryPanel>("CanvasLayer/InventoryPanel");
        var deathMenu = GetNode<DeathMenu>("CanvasLayer/DeathMenu");
        
        var shopPanel = GetNode<ShopPanel>("CanvasLayer/ShopPanel");
        shopPanel.Initialize(_player);
        
        _shopNpc = GetNode<ShopNPC>("Entities/ShopNPC");
        _shopNpc.Initialize(shopPanel);

        _combatManager = new CombatManager();
        AddChild(_combatManager);
        _combatManager.Initialize(_player, attackArea, seaCreatureScene);

        _fishingManager = new FishingManager();
        AddChild(_fishingManager);
        _fishingManager.Initialize(_player, waterLayer, fishingSprite, fishCaughtPopup, _combatManager);

        _uiManager = new UIManager();
        AddChild(_uiManager);
        _uiManager.Initialize(_player, inventoryPanel, deathMenu, shopPanel);

        _player.OnDeath += _uiManager.ShowDeathMenu;
    }
}