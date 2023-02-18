using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Components;

public class Player : Component
{
    public string Name { get; set; }
    public string Description { get; set; } 
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public float MoveSpeed { get; set; }
    public int PotraitIndex { get; set; }
    public Rectangle Source { get; set; }
    public Color Tint { get; set; }
    public Player(bool isActive = true) : base(isActive)
    {
    }
}