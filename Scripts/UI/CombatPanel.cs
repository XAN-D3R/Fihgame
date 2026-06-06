using Fihgame.Scripts.Model;
using Godot;

namespace Fihgame.Scripts.UI;

public partial class CombatPanel : Panel
{
    private Label _creatureName;
    private TextureRect _creatureSprite;
    private ProgressBar _creatureHpBar;
    private ProgressBar _playerHpBar;

    private SeaCreature _currentCreature;
    private Player.Player _player;

    private float _attackTimer = 0f;

    public override void _Ready()
    {
        _creatureName = GetNode<Label>("CreatureName");
        _creatureSprite = GetNode<TextureRect>("CreatureSprite");
        _creatureHpBar = GetNode<ProgressBar>("CreatureHpBar");
        _playerHpBar = GetNode<ProgressBar>("PlayerHpBar");
    }

    public override void _Process(double delta)
    {
        if (!Visible || _currentCreature == null) return;

        if (Input.IsActionJustPressed("attack"))
        {
            _currentCreature.TakeDamage(_player.Damage);
            GD.Print($"Player attacked {_currentCreature.Name} for {_player.Damage} damage! Creature HP: {_currentCreature.CurrentHp}/{_currentCreature.MaxHp}");
            UpdateHpBars();

            if (!_currentCreature.IsAlive)
            {
                EndCombat(true);
                return;
            }
        }

        _attackTimer -= (float)delta;
        if (_attackTimer <= 0f)
        {
            _player.TakeDamage(_currentCreature.Damage);
            _attackTimer = _currentCreature.AttackSpeed;
            UpdateHpBars();

            if (_player.CurrentHp <= 0)
            {
                EndCombat(false);
                return;
            }
        }
    }

    public void StartCombat(SeaCreature creature, Player.Player player)
    {
        _currentCreature = creature;
        _player = player;
        _attackTimer = creature.AttackSpeed;

        _creatureName.Text = creature.Name;
        _creatureSprite.Texture = GD.Load<Texture2D>(creature.SpritePath);

        UpdateHpBars();
        Visible = true;
    }

    private void UpdateHpBars()
    {
        _creatureHpBar.MaxValue = _currentCreature.MaxHp;
        _creatureHpBar.Value = _currentCreature.CurrentHp;

        _playerHpBar.MaxValue = _player.MaxHp;
        _playerHpBar.Value = _player.CurrentHp;
    }

    private void EndCombat(bool playerWon)
    {
        Visible = false;
        _currentCreature = null;

        if (playerWon)
            GD.Print("You defeated the creature!");
        else
            GD.Print("You were defeated!");
    }
}