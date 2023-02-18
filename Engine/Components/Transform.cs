using Microsoft.Xna.Framework;

namespace Engine.Components;

public class Transform : Component
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Layer { get; set; }
    public Rectangle Bounds => new (Position.ToPoint(), Size.ToPoint());
    public Transform(bool isActive = true) : base(isActive) { }
}