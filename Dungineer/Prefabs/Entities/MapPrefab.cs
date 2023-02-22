using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Prefabs.Entities;

public class MapPrefab : IPrefab<Entity>
{
    public Entity Instantiate(BaseGame game)
    {
        int mapWidth = 27;
        int mapHeight = 28;
        var groundTiles = new Tile[mapWidth, mapHeight];
        var objectTiles = new List<Tile>();
        for (int x = 0; x < groundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < groundTiles.GetLength(1); y++)
            {
                groundTiles[x, y] = new Tile
                {
                    X = x,
                    Y = y,
                    Tint = Color.White,
                    Type = TileType.DungeonFloor
                };


                //border
                if (x == 0 || y == 0 || x == groundTiles.GetLength(0) - 1 || y == groundTiles.GetLength(1) - 1)
                {
                    objectTiles.Add(new Tile
                    {
                        X = x,
                        Y = y,
                        Tint = Color.White,
                        Type = TileType.DungeonWall
                    });
                }
                else
                {
                    var rand = game.Rand.NextDouble();

                    if (rand <= 0.2f)
                    {
                        objectTiles.Add(new Tile
                        {
                            X = x,
                            Y = y,
                            Tint = Color.White,
                            Type = TileType.DungeonWall
                        });
                    }
                    else if (rand <= 0.25f)
                    {
                        objectTiles.Add(new Tile
                        {
                            X = x,
                            Y = y,
                            Tint = Color.White,
                            Type = TileType.DungeonFloorHole
                        });
                    }
                }


            }
        }
        var map = new Map
        {
            GroundTiles = groundTiles,
            ObjectTiles = objectTiles
        };


        var ent = new Entity()
            .With(map)
            .WithTag("Map");

        return ent;
    }
}