using Fihgame.Scripts.Model;
using Fihgame.Scripts.UI;
using Godot;

namespace Fihgame.Scripts.Player.Managers;

public partial class FishingManager : Node
{
    private Scripts.Player.Player _player;
    private TileMapLayer _waterLayer;
    private Sprite2D _fishingSprite;
    private FishCaughtPopup _fishCaughtPopup;
    private CombatManager _combatManager;

    private bool _isFishing = false;
    private float _fishingTimer = 0f;
    

    public float FishingWaitTime = 2.5f;

    public void Initialize(Scripts.Player.Player player, TileMapLayer waterLayer,
                           Sprite2D fishingSprite, FishCaughtPopup popup,
                           CombatManager combatManager)
    {
        _player = player;
        _waterLayer = waterLayer;
        _fishingSprite = fishingSprite;
        _fishCaughtPopup = popup;
        _combatManager = combatManager;

        _player.OnFishPressed += HandleFishPressed;
    }

    public override void _Process(double delta)
    {
        if (!_isFishing) return;

        _fishingTimer -= (float)delta;
        if (_fishingTimer <= 0f)
            CatchFish();
    }

    private void HandleFishPressed()
    {
        if (_isFishing) return;

        if (IsFacingWater())
            StartFishing();
        else
            GD.Print("Not facing water!");
    }

    private void StartFishing()
    {
        _isFishing = true;
        _player.IsBusy = true;
        _fishingTimer = FishingWaitTime;
        _fishingSprite.Visible = true;
        UpdateFishingRodPosition();
        _player.StopWalkingAnimation();
        GD.Print("Rod cast... waiting for fish!");
    }

    private void CatchFish()
    {
        _isFishing = false;
        _player.IsBusy = false;
        _fishingSprite.Visible = false;

        CatchableEntity caught = EntityFactory.CreateRandom();

        if (caught is SeaCreature creature)
        {
            _combatManager.StartCombat(creature);
        }
        else if (caught is Fish fish)
        {
            var fishItem = new FishItem(fish.Name, fish.SpritePath, fish.Description, fish.Rarity, fish.SellPrice);
            _player.Inventory.AddItem(fishItem);
            _player.Stats.AddFishCaught();
            _fishCaughtPopup.ShowFish(fish);
        }
    }

    private void UpdateFishingRodPosition()
    {
        switch (_player.LastDirection)
        {
            case "right": _fishingSprite.Position = new Vector2(8, 0);  _fishingSprite.FlipH = false; break;
            case "left":  _fishingSprite.Position = new Vector2(-8, 0); _fishingSprite.FlipH = true;  break;
            case "down":  _fishingSprite.Position = new Vector2(0, 8);  _fishingSprite.FlipH = false; break;
            case "up":    _fishingSprite.Position = new Vector2(0, -8); _fishingSprite.FlipH = false; break;
        }
        _fishingSprite.Rotation = 0f;
    }

    private bool IsFacingWater()
    {
        Vector2 checkPosition = _player.GlobalPosition;
        const int distance = 32;

        switch (_player.LastDirection)
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