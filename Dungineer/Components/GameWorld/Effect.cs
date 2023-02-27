using Engine;
using System;

namespace Dungineer.Components.GameWorld;

public class Effect : Component
{
    public Action Passive { get; set; }
    public Action Active { get; set; }
}