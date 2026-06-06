using System.Collections.Generic;
using Fihgame.Scripts.Model;
using Fihgame.Scripts.World;
using Godot;

namespace Fihgame.Scripts.Player.Managers;

public partial class CombatManager : Node
{
    private Player _player;
    private Area2D _attackArea;
    private PackedScene _seaCreatureScene;
    
    private Node2D _weaponPivot;
    private Sprite2D _weaponSprite;
    
    private float _swingTimer = 0f;
    private float _swingDuration = 0.3f;
    private float _swingAngle = 90f;

    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private float _attackDuration = 0.5f;
    
    public void Initialize(Player player, Area2D attackArea, PackedScene seaCreatureScene)
    {
        _player = player;
        _attackArea = attackArea;
        _seaCreatureScene = seaCreatureScene;
        
        _weaponPivot = _player.GetNode<Node2D>("WeaponPivot");
        _weaponSprite = _weaponPivot.GetNode<Sprite2D>("WeaponSprite");
        _weaponPivot.Visible = false;

        _attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

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
            _weaponPivot.Visible = false;
        }

        _swingTimer -= (float)delta;
        float progress = 1f - Mathf.Clamp(_swingTimer / _swingDuration, 0f, 1f);
        float swing = Mathf.Lerp(_swingAngle / 2f, -_swingAngle / 2f, progress);

        switch (_player.LastDirection)
        {
            case "right": _weaponPivot.RotationDegrees = 90f  + swing; break;
            case "left":  _weaponPivot.RotationDegrees = -90f + swing; break;
            case "down":  _weaponPivot.RotationDegrees = 180f + swing; break;
            case "up":    _weaponPivot.RotationDegrees = 0f   + swing; break;
        }

        foreach (Area2D area in _attackArea.GetOverlappingAreas())
        {
            if (area.GetParent() is SeaCreatureEntity entity)
            {
                Vector2 knockbackDir = (area.GlobalPosition - _player.GlobalPosition).Normalized();
                entity.TakeDamage(_player.Damage, knockbackDir, _attackDuration);
            }
        }
    }

    private void HandleAttackPressed()
    {
        if (_isAttacking) return;

        UpdateAttackAreaPosition();
        var col = _attackArea.GetNode<CollisionShape2D>("CollisionShape2D");
    
        col.Disabled = true;
        col.Disabled = false;
    
        _isAttacking = true;
        _attackTimer = _attackDuration;
        _swingTimer = _swingDuration;
        _weaponPivot.Visible = true;
    }
    
    private void UpdateWeaponPosition()
    {
        switch (_player.LastDirection)
        {
            case "right": _weaponPivot.RotationDegrees = 90f;  break;
            case "left":  _weaponPivot.RotationDegrees = -90f; break;
            case "down":  _weaponPivot.RotationDegrees = 180f; break;
            case "up":    _weaponPivot.RotationDegrees = 0f;   break;
        }
    }

    public void StartCombat(SeaCreature creature)
    {
        SeaCreatureEntity entity = _seaCreatureScene.Instantiate<SeaCreatureEntity>();

        Vector2 spawnOffset = _player.LastDirection switch
        {
            "right" => new Vector2(-96, 0),
            "left"  => new Vector2(96, 0),
            "down"  => new Vector2(0, -96),
            "up"    => new Vector2(0, 96),
            _       => new Vector2(0, 96)
        };

        entity.GlobalPosition = _player.GlobalPosition + spawnOffset;
        entity.Initialize(creature, _player);
        _player.GetParent().AddChild(entity);

        GD.Print($"A {creature.Name} appeared!");
    }

    private void UpdateAttackAreaPosition()
    {
        var shape = _attackArea.GetNode<CollisionShape2D>("CollisionShape2D");
    
        switch (_player.LastDirection)
        {
            case "right": SetConeShape(shape, 0f);    _attackArea.Position = new Vector2(8, 0);  break;
            case "left":  SetConeShape(shape, 180f);  _attackArea.Position = new Vector2(-8, 0); break;
            case "down":  SetConeShape(shape, 90f);   _attackArea.Position = new Vector2(0, 8);  break;
            case "up":    SetConeShape(shape, -90f);  _attackArea.Position = new Vector2(0, -8); break;
        }
    }

    private void SetConeShape(CollisionShape2D shape, float angleDegrees)
    {
        float angleRad = Mathf.DegToRad(angleDegrees);
        float spread = Mathf.DegToRad(50f);
        float length = 32f;
        int segments = 8;

        var points = new Vector2[segments + 2];
        points[0] = Vector2.Zero;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = angleRad - spread + t * (spread * 2f);
            points[i + 1] = new Vector2(
                Mathf.Cos(currentAngle),
                Mathf.Sin(currentAngle)
            ) * length;
        }

        var polygon = new ConvexPolygonShape2D();
        polygon.Points = points;
        shape.Shape = polygon;
    }
}