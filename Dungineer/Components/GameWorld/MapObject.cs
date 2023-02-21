using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Components.GameWorld;

public class MapObject : Component
{
    public int MapX { get; set; }
    public int MapY { get; set; }
    public MapObjectType Type { get; set; }
    public Color Tint { get; set; }
    public float Scale { get; set; } = 1f;
    public MapObject(bool isActive = true) : base(isActive)
    {

    }
}