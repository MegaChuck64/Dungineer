using Microsoft.Xna.Framework;

namespace Engine.Components;

public class Sprite : Component
{
    public string TextureName { get; set; }
    public Color Tint { get; set; }    
    public Rectangle? Source { get; set; }
    public Sprite(Entity owner, bool isActive = true) : base(owner, isActive) { }
}