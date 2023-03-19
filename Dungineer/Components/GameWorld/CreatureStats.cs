using Engine;
using System;

namespace Dungineer.Components.GameWorld;

public class CreatureStats : Component, ICloneable
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Strength { get; set; }
    public float MoveSpeed { get; set; }
    public float SightRange { get; set; }
    //public float AttackRange { get; set; }
    public int Money { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}