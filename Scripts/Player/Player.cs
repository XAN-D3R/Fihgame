using System;
using Fihgame.Scripts.Data;
using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.Player;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 150f;
    [Export] public float SprintMultiplier = 1.75f;
    [Export] public float SprintDuration = 3f;    // sprint duration (in sec)
    [Export] public float SprintRechargeRate = 1f; // recharge time (in sec) WIP
    [Export] public int MaxHp = 100;
    [Export] public int CurrentHp = 100;
    [Export] public int Damage = 10;
    
    public Equipment Equipment = new Equipment();

    public string LastDirection = "down";
    public GameStats Stats = new GameStats();
    public Inventory Inventory = new Inventory();
    public int Coins = 0;

    public event Action OnFishPressed;
    public event Action OnAttackPressed;
    public event Action OnInventoryPressed;
    public event Action OnSprintHeld;
    public event Action OnDeath;

    private AnimatedSprite2D _sprite;
    private ProgressBar _hpBar;

    private float _healTimer = 0f;
    private float _healInterval = 2f;
    private int _healAmount = 5;
    private float _healDelay = 5f;
    private float _timeSinceDamage = 0f;

    private float _sprintStamina;
    private bool _isSprinting = false;
    
    public bool IsBusy = false;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _hpBar = GetNode<ProgressBar>("HpBar");
        _hpBar.MaxValue = MaxHp;
        _hpBar.Value = CurrentHp;
        _hpBar.Visible = false;

        _sprintStamina = SprintDuration;
        AddStartingItems();
    }
    
    private void AddStartingItems()
    {
        foreach (var item in DataLoader.LoadStartingInventory())
            Inventory.AddItem(item);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleHealing((float)delta);
        HandleSprint((float)delta);
        Stats.UpdateTime((float)delta);

        if (!IsBusy)
        {
            if (Input.IsActionJustPressed("fish"))      OnFishPressed?.Invoke();
            if (Input.IsActionJustPressed("attack"))    OnAttackPressed?.Invoke();
            if (Input.IsActionJustPressed("inventory")) OnInventoryPressed?.Invoke();

            HandleMovement();
        }
    }

    private void HandleSprint(float delta)
    {
        bool sprintPressed = Input.IsActionPressed("sprint");
        bool isMoving = Velocity != Vector2.Zero;

        if (sprintPressed && isMoving && _sprintStamina > 0f)
        {
            _isSprinting = true;
            _sprintStamina = Mathf.Max(0f, _sprintStamina - delta);
            OnSprintHeld?.Invoke();
        }
        else
        {
            _isSprinting = false;
            _sprintStamina = Mathf.Min(SprintDuration, _sprintStamina + delta * SprintRechargeRate);
        }
    }

    private void HandleMovement()
    {
        if (IsBusy) return;

        Vector2 direction = Vector2.Zero;
    
        if (Input.IsActionPressed("move_right")) direction.X += 1;
        if (Input.IsActionPressed("move_left"))  direction.X -= 1;
        if (Input.IsActionPressed("move_down"))  direction.Y += 1;
        if (Input.IsActionPressed("move_up"))    direction.Y -= 1;

        if      (direction.X > 0) LastDirection = "right";
        else if (direction.X < 0) LastDirection = "left";
        else if (direction.Y > 0) LastDirection = "down";
        else if (direction.Y < 0) LastDirection = "up";

        float currentSpeed = _isSprinting ? Speed * SprintMultiplier : Speed;
        Velocity = direction.Normalized() * currentSpeed;
        MoveAndSlide();

        Vector2 realVelocity = GetRealVelocity();
        float expectedSpeed = direction == Vector2.Zero ? 0f : currentSpeed;
        bool blocked = direction != Vector2.Zero && realVelocity.Length() < expectedSpeed * 0.5f;
        
        if (direction == Vector2.Zero || blocked)
        {
            _sprite.Play($"idle_{LastDirection}");
            _sprite.Stop();
        }
        else
        {
            _sprite.Play($"walk_{LastDirection}");
        }
    }

    private void HandleHealing(float delta)
    {
        _timeSinceDamage += delta;

        if (CurrentHp < MaxHp && _timeSinceDamage >= _healDelay)
        {
            _healTimer += delta;
            if (_healTimer >= _healInterval)
            {
                CurrentHp = Mathf.Min(MaxHp, CurrentHp + _healAmount);
                UpdateHpBar();
                _healTimer = 0f;
            }
        }
    }

    public void TakeDamage(int damage, string attackerName = "Unknown")
    {
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
        _timeSinceDamage = 0f;
        _healTimer = 0f;
        UpdateHpBar();
        
        if (CurrentHp <= 0)
        {
            Stats.SetKilledBy(attackerName);
            OnDeath?.Invoke();
        }
    }

    private void UpdateHpBar()
    {
        _hpBar.MaxValue = MaxHp;
        _hpBar.Value = CurrentHp;
        _hpBar.Visible = CurrentHp < MaxHp;
    }

    // Stamina bar WIP
    public float GetSprintStaminaPercent() => _sprintStamina / SprintDuration;
    
    public void StopWalkingAnimation()
    {
        _sprite.Play($"idle_{LastDirection}");
        _sprite.Stop();
    }
}