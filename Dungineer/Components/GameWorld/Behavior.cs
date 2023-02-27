using Engine;
using System;

namespace Dungineer.Components.GameWorld;

public class Behavior : Component
{
    public Action Perform { get; set; }
}