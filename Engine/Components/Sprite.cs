using Microsoft.Xna.Framework;

namespace Engine.Components;

public class Sprite : Component
{
    public string TextureName { get; set; }
    public Color Tint { get; set; }    
    public Vector2 Offset { get; set; }
    public Rectangle? Source { get; set; }
    public Sprite(bool isActive = true) : base(isActive) { }
}