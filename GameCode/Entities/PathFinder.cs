using AStar;
using AStar.Options;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCode.Entities;

public class PathFinder : Entity
{
    public TileMap Map { get; set; }
    public List<(int x, int y)> Path { get; set; }
    public int CurrentStep { get; set; }

    public bool showDebug = false;
    public Color debugTint;

    public PathFinder(BaseGame game, TileMap map) : base(game)
    {
        Map = map;
        debugTint = new Color(88, 33, 33);
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

        var tiles = new short[Map.Width, Map.Height];
        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                tiles[x, y] =
                    (Map.TryGetTile(x, y)?.HasCollider ?? false) ||
                    (Map.TryGetTileObject(x, y)?.HasCollider ?? false) ?
                    (short)0 : (short)1;
            }
        }

        var grid = new WorldGrid(tiles);
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


    public bool IsEnd()
    {
        if (Path.Count > 0)
        {
            if (CurrentStep >= Path.Count - 1)
            {
                return true;
            }
        }

        return false;
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        if (showDebug)
        {
            if (Path == null) return;

            foreach (var (x, y) in Path)
            {
                sb.FillRectangle(new RectangleF(x * Map.TileSize, y * Map.TileSize, Map.TileSize, Map.TileSize), debugTint, 0.25f);
            }
        }
    }

}