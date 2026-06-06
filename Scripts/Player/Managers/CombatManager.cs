using Fihgame.Scripts.Model;
using Fihgame.Scripts.World;
using Godot;

namespace Fihgame.Scripts.Player.Managers;

public partial class CombatManager : Node
{
    private Scripts.Player.Player _player;
    private Area2D _attackArea;
    private PackedScene _seaCreatureScene;

    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private float _attackDuration = 0.5f;

    public void Initialize(Scripts.Player.Player player, Area2D attackArea, PackedScene seaCreatureScene)
    {
        _player = player;
        _attackArea = attackArea;
        _seaCreatureScene = seaCreatureScene;

        _attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        _attackArea.AreaEntered += OnAttackAreaEntered;

        _player.OnAttackPressed += HandleAttackPressed;
    }

    public override void _Process(double delta)
    {
        if (!_isAttacking) return;

        _attackTimer -= (float)delta;
        if (_attackTimer <= 0f)
        {
            _isAttacking = false;
            _attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        }
    }

    private void HandleAttackPressed()
    {
        if (_isAttacking) return;

        UpdateAttackAreaPosition();
        _attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        _isAttacking = true;
        _attackTimer = _attackDuration;
    }

    private void OnAttackAreaEntered(Area2D area)
    {
        if (!_isAttacking) return;

        if (area.GetParent() is SeaCreatureEntity entity)
        {
            entity.TakeDamage(_player.Damage);
            GD.Print($"Player attacked for {_player.Damage} damage!");
        }
    }

    public void StartCombat(SeaCreature creature)
    {
        SeaCreatureEntity entity = _seaCreatureScene.Instantiate<SeaCreatureEntity>();

        Vector2 spawnOffset = _player.LastDirection switch
        {
            "right" => new Vector2(-48, 0),
            "left"  => new Vector2(48, 0),
            "down"  => new Vector2(0, -48),
            "up"    => new Vector2(0, 48),
            _       => new Vector2(0, 48)
        };

        entity.GlobalPosition = _player.GlobalPosition + spawnOffset;
        entity.Initialize(creature, _player);
        _player.GetParent().AddChild(entity);

        GD.Print($"A {creature.Name} appeared!");
    }

    private void UpdateAttackAreaPosition()
    {
        switch (_player.LastDirection)
        {
            case "right": _attackArea.Position = new Vector2(24, 0);  break;
            case "left":  _attackArea.Position = new Vector2(-24, 0); break;
            case "down":  _attackArea.Position = new Vector2(0, 24);  break;
            case "up":    _attackArea.Position = new Vector2(0, -24); break;
        }
    }
}