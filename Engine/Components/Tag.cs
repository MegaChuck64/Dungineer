using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Components;

public class Tag : Component
{
    public string Value { get; set; }
    public Tag(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}