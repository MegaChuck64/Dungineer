using Dungineer.Models;
using Engine;

namespace Dungineer.Components.GameWorld;

public class Wardrobe : Component
{
    public WardrobeType? BodySlot { get; set; }

    public WardrobeType? HatSlot { get; set; }
}