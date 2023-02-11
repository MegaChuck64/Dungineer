using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Components;

public class Text : Component
{
    public string FontName { get; set; }
    public string Content { get; set; }
    public Vector2 Offset { get; set; }
    public Color Tint { get; set; }

    public Text(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}