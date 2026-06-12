using System;
using Godot;

namespace Fihgame.Scripts.Model;

public class SeaCreature : CatchableEntity
{
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Damage { get; set; }
    public float AttackSpeed { get; set; } // Seconden tussen aanvallen
    
    public DroppedItem[] Drops { get; set; }

    public SeaCreature(string name, string spritePath, string description,
        int maxHp, int damage, float attackSpeed = 1f,
        DroppedItem[] drops = null, float weight = 1f) : base(name, spritePath, description, weight)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
        Damage = damage;
        AttackSpeed = attackSpeed;
        Drops = drops ?? Array.Empty<DroppedItem>();
    }

    public bool IsAlive => CurrentHp > 0;

    public void TakeDamage(int damage)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
    }
}