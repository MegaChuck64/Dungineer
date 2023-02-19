using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Dungineer.Prefabs;

public class MapPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {
        int mapWidth = 27;
        int mapHeight = 20;
        var groundTiles = new Tile[mapWidth, mapHeight];
        var objectTiles = new List<Tile>();
        for (int x = 0; x < groundTiles.GetLength(0); x++)
        {
            for (int y= 0; y < groundTiles.GetLength(1); y++)
            {
                groundTiles[x, y] = new Tile
                {
                    X = x,
                    Y = y,
                    Tint = Color.White,
                    Type = TileType.DungeonFloor
                };
                
                var rand = game.Rand.NextDouble();

                if (rand > 0.97f)
                    objectTiles.Add(new Tile
                    {
                        X = x,
                        Y = y,
                        Tint = Color.White,
                        Type = TileType.DungeonFloorHole
                    });
                else if (rand > 0.6f)
                    objectTiles.Add(new Tile
                    {
                        X = x,
                        Y = y,
                        Tint = Color.White,
                        Type = TileType.DungeonWall
                    });
                else if (rand > 0.57f)
                    objectTiles.Add(new Tile
                    {
                        X = x,
                        Y = y,
                        Tint = Color.White,
                        Type = TileType.Ghost
                    });

            }
        }
        var map = new Map
        {
            GroundTiles = groundTiles,
            ObjectTiles = objectTiles
        };

        var trn = new Transform
        {
            Position = new Vector2(),
            Layer = 0.5f,
        };


        var ent = new Entity(game)
            .With(map)
            .With(trn);

        return ent;
    }
}