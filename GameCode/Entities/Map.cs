﻿
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Entities;


public class Map
{
    public MapTile[,] Tiles { get; private set; }
    public List<MapObject> Objects { get; private set; }
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    public MapTile GetTile(int x, int y) =>
        (x >= 0 && x < Width && y >= 0 && y < Height) 
        ? Tiles[x, y] : null;

    public IEnumerable<MapObject> GetMapObjects(int x, int y) =>
        Objects.Where(o => 
        o.X == x && 
        o.Y == y);

    public IEnumerable<MapObject> GetObjectsInRadius(int x, int y, int r) =>
        Objects.Where(
            o => Vector2.Distance(
                new Vector2(x, y), 
                new Vector2(o.X, o.Y)) 
            <= r);

    public IEnumerable<MapTile> GetAdjacentTiles(int x, int y, bool includeDiagonals)
    {
        var tiles = new List<MapTile>();

        if (x > 0) tiles.Add(GetTile(x - 1, y));
        if (x < Width - 1) tiles.Add(GetTile(x + 1, y));
        if (y > 0) tiles.Add(GetTile(x, y - 1));
        if (y < Height - 1) tiles.Add(GetTile(x, y + 1));

        if (includeDiagonals)
        {
            if (x > 0 && y > 0) tiles.Add(GetTile(x - 1, y - 1));
            if (x < Width - 1 && y < Width - 1) tiles.Add(GetTile(x + 1, y + 1));
            if (x > 0 && y < Width - 1) tiles.Add(GetTile(x - 1, y + 1));
            if (x < Width - 1 && y >= 0) tiles.Add(GetTile(x + 1, y - 1));
        }

        return tiles;
    }
    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new MapTile[Width, Height];
        Objects = new List<MapObject>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new MapTile()
                {
                    X = x, 
                    Y = y,
                    Name = "Unknown",
                    Solid = false
                };
            }
        }
    }
}

public class MapTile
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool Solid { get; set; }
}

public class MapObject : MapTile
{
    public string Description { get; set; }
    
    /// <summary>
    /// Tiles Per Tick
    /// </summary>
    public int Speed { get; set; }
    /// <summary>
    /// 1 used every tick after moving
    /// </summary>
    public int Stamina { get; set; }

    /// <summary>
    /// Stamina regained per tick
    /// </summary>
    public float StaminaRegen { get; set; }



    public int Health { get; set; }
    /// <summary>
    /// Percent 0-1. .5 means you take half damage 
    /// 1 means you take none, over 1 gives damage to attacker 
    /// </summary>
    public float Armor { get; set; }
    /// <summary>
    /// Health regained per tick
    /// </summary>
    public float HealthRegen { get; set; }
}
