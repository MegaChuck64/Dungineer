using Engine;
using System.Collections.Generic;


namespace Dungineer.Components.GameWorld;

public class Spell : Component
{
    public float Range { get; set; }
    public List<Effect> Effects { get; set; }
    public int Cost { get; set; }
}