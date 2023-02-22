using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Components.GameWorld;

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

        if (mapObjects.Any(c => c.MapX == x && c.MapY == y && !Settings.MapObjectAtlas[c.Type].Collectable))
            return false;

        return true;
    }

    public IEnumerable<(int x, int y)> GetAdjacentEmptyTiles(int x, int y, bool includeDiagonals, params MapObject[] mapObjects)
    {
        var adjacent = GetAdjacentTiles(x, y, includeDiagonals);
        var filterMapObjects = adjacent
            .Where(t =>
            !Settings.TileAtlas[GroundTiles[t.x, t.y].Type].Solid &&
            !ObjectTiles.Any(v => v.X == t.x && v.Y == t.y && Settings.TileAtlas[v.Type].Solid) &&
            !mapObjects.Any(g => g.MapX == t.x && g.MapY == t.y && !Settings.MapObjectAtlas[g.Type].Collectable)).ToList();

        return filterMapObjects;
    }
    public List<(int x, int y)> GetAdjacentTiles(int x, int y, bool includeDiagonals = false)
    {
        var adjacent = new List<(int, int)>();

        if (y - 1 >= 0) adjacent.Add((x, y - 1));
        if (y + 1 < GroundTiles.GetLength(1)) adjacent.Add((x, y + 1));
        if (x - 1 >= 0) adjacent.Add((x - 1, y));
        if (x + 1 < GroundTiles.GetLength(0)) adjacent.Add((x + 1, y));


        if (includeDiagonals)
        {
            if (x - 1 >= 0 && y - 1 >= 0)
                adjacent.Add((x - 1, y - 1));
            if (x - 1 >= 0 && y + 1 < GroundTiles.GetLength(1))
                adjacent.Add((x - 1, y + 1));
            if (x + 1 < GroundTiles.GetLength(0) && y - 1 >= 0)
                adjacent.Add((x + 1, y - 1));
            if (x + 1 < GroundTiles.GetLength(0) && y + 1 < GroundTiles.GetLength(1))
                adjacent.Add((x + 1, y + 1));
        }

        return adjacent;
    }

}

public class Tile
{
    public TileType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Color Tint { get; set; }
}
