using Dungineer.Components.GameWorld;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Behaviors;

public class TargetMovement : IBehavior
{
    public Point Target { get; set; }
    public TargetMovement(Point target)
    {
        Target = target;
    }
    public void Perform(Entity ent)
    {
        var map = SceneManager.ComponentsOfType<Map>().FirstOrDefault();
        var mapObjs = SceneManager.ComponentsOfType<MapObject>().ToArray();
        var mapObj = ent.GetComponent<MapObject>();

        var path = GetPath(
            new Point(mapObj.MapX, mapObj.MapY),
                Target,
                map,
                mapObjs);

        if (path != null && path.Count > 0)
        {
            var nextStep = path.First();
            mapObj.MapX = nextStep.X;
            mapObj.MapY = nextStep.Y;            
        }
    }

    public static List<Point> GetPath(Point start, Point end, Map map, params MapObject[] mapObjects)
    {
        var grid = new bool[map.GroundTiles.GetLength(0), map.GroundTiles.GetLength(1)];

        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var groundTile = Settings.TileAtlas[map.GroundTiles[x, y].Type];
                var objectTiles = map.ObjectTiles.Where(t => t.X == x && t.Y == y).Select(v => Settings.TileAtlas[v.Type]);

                var hasGroundCollision = groundTile.Solid;
                var hasObjectCollision = objectTiles.Any(y => y.Solid);
                var hasItemCollision = mapObjects.Any(g => g.MapX == x && g.MapY == y && !Settings.MapObjectAtlas[g.Type].Collectable);

                grid[x, y] = !hasGroundCollision && !hasObjectCollision && !hasItemCollision;
            }
        }

        var pathFinder = new PathFinder(new(start, end, grid));

        return pathFinder.FindPath();

    }
}