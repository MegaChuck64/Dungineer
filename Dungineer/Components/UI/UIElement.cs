using Dungineer.Systems;
using Engine;
using Microsoft.Xna.Framework;
using System;

namespace Dungineer.Components.UI;

public class UIElement : Component
{
    public Point Position { get; set; }

    public Point Size { get; set; }

    public Rectangle Bounds => new(Position, Size);
    public Action OnMouseEnter { get; set; }

    public Action OnMouseLeave { get; set; }

    public Action<MouseButton> OnMousePressed { get; set; }

    public Action<MouseButton> OnMouseReleased { get; set; }
}