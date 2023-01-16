using AStar.Options;
using Engine;
using Microsoft.Xna.Framework.Graphics;
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
    public PathFinder(BaseGame game, TileMap map) : base(game)
    {
        Map = map;
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
                //tiles[x,y] = (Map.TryGetTile(x,y)?.HasCollider ?? false) || Map.TileObj
            }
        }
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
    }

}