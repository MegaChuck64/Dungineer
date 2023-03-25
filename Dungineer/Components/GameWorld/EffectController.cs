using Dungineer.Behaviors.Effects;
using Engine;
using System.Collections.Generic;

namespace Dungineer.Components.GameWorld;

public class EffectController : Component
{
    public List<IEffect> Effects { get; set; } = new();
}