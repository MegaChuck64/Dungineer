using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Components.GameWorld;

public class MapObject : Component
{
    public int MapX { get; set; }
    public int MapY { get; set; }
    public MapObjectType Type { get; set; }
    public Color Tint { get; set; }
    public float Scale { get; set; } = 1f;

}