using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Components;

public class Map : Component
{
    public Tile[,] GroundTiles { get; set; }
    public List<Tile> ObjectTiles { get; set; }

    public (int x, int y) GetRandomEmptyTile(BaseGame game, params MapObject[] mapObjects)
    {
        int randX;
        int randY;

        do
        {
            randX = game.Rand.Next(0, GroundTiles.GetLength(0));
            randY = game.Rand.Next(0, GroundTiles.GetLength(1));
        } while (!IsEmpty(randX, randY, mapObjects));

        return (randX, randY);
    }

    public bool IsEmpty(int x, int y, params MapObject[] mapObjects)
    {
        if (Settings.TileAtlas[GroundTiles[x, y].Type].Solid)
            return false;

        if (ObjectTiles.Any(t => t.X == x && t.Y == y && Settings.TileAtlas[t.Type].Solid))
            return false;

        if (mapObjects.Any(c => c.MapX == x && c.MapY == y))
            return false;

        return true;
    }
    public Map(bool isActive = true) : base(isActive)
    {

    }
}

public class Tile
{
    public TileType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Color Tint { get; set; }
}
