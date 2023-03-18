using Dungineer.Behaviors;
using Engine;
using System.Collections.Generic;

namespace Dungineer.Components.GameWorld;

public class SpellBook : Component
{
    public int selectedSpell = -1;
    public List<ISpell> Spells { get; set; } = new ();
}