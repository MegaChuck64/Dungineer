using Engine;

namespace Dungineer.Components.GameWorld;

public class CreatureStats : Component
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public float MoveSpeed { get; set; }

    public float SightRange { get; set; }

    public int Money { get; set; }
}