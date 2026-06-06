using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.World;

public partial class SeaCreatureEntity : CharacterBody2D
{
    private Sprite2D _sprite;
    private NavigationAgent2D _navigationAgent;
    private ProgressBar _hpBar;

    private SeaCreature _data;
    private Player.Player _player;
    
    private Area2D _hurtBox;

    private float _attackTimer = 0f;
    private float _attackRange = 40;
    
    private float _stopDistance = 48;
    
    private PackedScene _droppedItemScene = GD.Load<PackedScene>("res://Scenes/Entities/DroppedItemEntity.tscn");

    public override void _Ready()
    {
        
    }

    public void Initialize(SeaCreature data, Player.Player player)
    {
        _hurtBox = GetNode<Area2D>("HurtBox");
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _hpBar = GetNode<ProgressBar>("HpBar");
        
        _data = data;
        _player = player;

        _sprite.Texture = GD.Load<Texture2D>(data.SpritePath);

        _hpBar.MaxValue = data.MaxHp;
        _hpBar.Value = data.CurrentHp;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_data == null || _player == null) return;

        float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (distanceToPlayer > _stopDistance)
        {
            _navigationAgent.TargetPosition = _player.GlobalPosition;
            Vector2 direction = (_navigationAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            Velocity = direction * 60f;
        }
        else
        {
            Velocity = Vector2.Zero;
        
            _attackTimer -= (float)delta;
            if (_attackTimer <= 0f)
            {
                _player.TakeDamage(_data.Damage, _data.Name);
                _attackTimer = _data.AttackSpeed;
                GD.Print($"Creature attacked player for {_data.Damage} damage!");
            }
        }

        MoveAndSlide();
    }

    public void TakeDamage(int damage)
    {
        _data.TakeDamage(damage);
        _hpBar.Value = _data.CurrentHp;

        if (!_data.IsAlive)
        {
            SpawnDrops();
            _player.Stats.AddCreatureDefeated();
            GD.Print($"{_data.Name} defeated!");
            QueueFree();
        }
    }

    private void SpawnDrops()
    {
        foreach (DroppedItem drop in _data.Drops)
        {
            DroppedItemEntity entity = _droppedItemScene.Instantiate<DroppedItemEntity>();
            entity.GlobalPosition = GlobalPosition + new Vector2(
                GD.RandRange(-16, 16), 
                GD.RandRange(-16, 16)
            );
            GetParent().AddChild(entity);
            entity.Initialize(drop);
        }
    }
}