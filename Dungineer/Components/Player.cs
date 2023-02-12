using Engine;

namespace Dungineer.Components;

public class Player : Component
{
    public string Name { get; set; }
    public string Description { get; set; } 
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int PotraitIndex { get; set; }
    public Player(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}