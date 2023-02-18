using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Components;

public class Map : Component
{
    public Tile[,] GroundTiles { get; set; }
    public List<Tile> ObjectTiles { get; set; }
    public Map(bool isActive = true) : base(isActive)
    {

    }
}

public struct Tile
{
    public TileType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Color Tint { get; set; }
}
