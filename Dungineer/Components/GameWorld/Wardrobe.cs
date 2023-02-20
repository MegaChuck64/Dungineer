using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Components.GameWorld;

public class Wardrobe : Component
{
    public WardrobeType? BodySlot { get; set; }

    public WardrobeType? HatSlot { get; set; }
}