using Dungineer.Behaviors;
using Engine;
using System.Collections.Generic;

namespace Dungineer.Components.GameWorld;

public class BehaviorController : Component
{
    public List<IBehavior> Behaviors { get; set; } = new ();
}