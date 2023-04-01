using System;
using Engine;
using Microsoft.Xna.Framework;

namespace MonogameUI;

public class UIElement : Component
{
    public Point Position { get; set; }

    public Point Size { get; set; }

    public Color Color { get; set; } = Color.White;
    public Rectangle Bounds => new(Position, Size);
    public Action OnMouseEnter { get; set; }

    public Action OnMouseLeave { get; set; }

    public Action<MouseButton> OnMousePressed { get; set; }

    public Action<MouseButton> OnMouseReleased { get; set; }
}