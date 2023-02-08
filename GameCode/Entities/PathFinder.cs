using AStar;
using AStar.Options;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Entities;

public class PathFinder : Component
{
    public int TileSize { get; private set; }
    public short[,] Map { get; set; }
    public List<(int x, int y)> Path { get; set; }
    public int CurrentStep { get; set; }

    public Color debugTint;

    public PathFinder(Entity entity, short[,] map, int tileSize) : base(entity)
    {
        TileSize = tileSize;
        Map = map;
        debugTint = new Color(88, 33, 33, 100);
    }

    public void Clear()
    {
        Path = new List<(int x, int y)>();
    }

    public void CreatePath(
        (int x, int y) startPos, 
        (int x, int y) endPos, 
        bool useDiagonals = false, 
        bool punishDirectionChange = true)
    {
        var pathFinderOptions = new PathFinderOptions
        {
            PunishChangeDirection = punishDirectionChange,
            UseDiagonals = useDiagonals,
        };

        //var tiles = new short[Map.GetLength(0), Map.GetLength(1)];
        //for (int x = 0; x < Map.GetLength(0); x++)
        //{
        //    for (int y = 0; y < Map.GetLength(1); y++)
        //    //{
        //    //    tiles[x, y] =
        //    //        (Map.TryGetTile(x, y)?.HasCollider ?? false) ||
        //    //        (Map.TryGetTileObject(x, y)?.HasCollider ?? false) ?
        //    { 
        //        var j = 0 == 0 ? (short)0 : (short)1;
        //        //collision check on generic type
        //    }
        //}

        var grid = new WorldGrid(Map);
        var pathfinder = new AStar.PathFinder(grid, pathFinderOptions);

        var path = pathfinder.FindPath(new Position(startPos.x, startPos.y), new Position(endPos.x, endPos.y))
            .Select(p => (p.Row, p.Column))
            .ToList();

        CurrentStep = 0;
        Path = path ?? Path;
    }

    public bool TryStepForward(out (int x, int y)? newPosition)
    {
        if (IsEnd())
        {
            if (Path.Count > 0) newPosition = Path.Last();
            else newPosition = null;
            return false;
        }
        else
        {
            CurrentStep++;
            if (Path.Count > CurrentStep)
            {
                newPosition = Path[CurrentStep];
                return true;
            }
            else
            {
                newPosition = null;
                return false;
            }
        }
    }


    public bool IsEnd() =>
        Path.Count > 0 && CurrentStep >= Path.Count - 1;

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        if (Owner.Game.Debug)
        {
            if (Path == null) return;

            foreach (var (x, y) in Path)
            {
                sb.FillRectangle(new RectangleF(x * TileSize, y * TileSize, TileSize, TileSize), debugTint, 0.15f);
            }
        }
    }

}