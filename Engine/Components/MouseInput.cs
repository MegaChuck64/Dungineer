using Engine.Systems;
using System;

namespace Engine.Components;

public class MouseInput : Component
{
    public Action OnMouseEnter;

    public Action OnMouseLeave;

    public Action<MouseButton> OnMousePressed;

    public Action<MouseButton> OnMouseReleased;

    public MouseInput(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}