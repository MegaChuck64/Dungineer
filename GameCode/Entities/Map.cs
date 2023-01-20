
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Entities;


public class Map
{
    public MapTile[,] Tiles { get; private set; }
    public List<MapObject> Objects { get; private set; }
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    public FastRandom Rand { get; set; }
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

    public short[,] ToShortCollisionMap((int x, int y)? exclude = null)
    {
        var shMap = new short[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (exclude != null && x == exclude.Value.x && y == exclude.Value.y)
                {
                    shMap[x, y] = 1;
                }
                else
                {
                    shMap[x, y] = 
                        (Tiles[x, y].Solid || GetMapObjects(x, y).Any(c => c.Solid)) ? (short)0 : (short)1;
                }
            }
        }

        return shMap;
    }
    public (int x, int y) GetRandomEmptyTile()
    {
        var randX = Rand.Next(0, Width - 1);
        var randY = Rand.Next(0, Height - 1);
        while (GetMapObjects(randX, randY).Any())
        {
            randX = Rand.Next(0, Width - 1);
            randY = Rand.Next(0, Height - 1);
        }

        return (randX, randY);
    }
    public Map(int width, int height, FastRandom rand, float treePercentage = 0.1f)
    {
        Rand = rand;
        Width = width;
        Height = height;
        Tiles = new MapTile[Width, Height];
        Objects = new List<MapObject>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new MapTile
                {
                    X = x, 
                    Y = y,
                    Name = "Grass",
                    Solid = false
                };

                if (Rand.NextSingle(0f, 1f) <= treePercentage)
                {
                    Objects.Add(new MapObject
                    {
                        X = x,
                        Y = y,
                        Name = "Pine Tree",
                        Description = "Simple Pine Tree",
                        Solid = true,
                        Speed = 0,
                        Armor = 0.25f,
                        Health = 10,
                        MaxHealth = 10,
                        HealthRegen = .5f,
                        Stamina = 1,
                        MaxStamina = 1,
                        StaminaRegen = 0f,                         
                    });
                }
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
    public int MaxStamina { get; set; }
    /// <summary>
    /// Stamina regained per tick
    /// </summary>
    public float StaminaRegen { get; set; }



    public int Health { get; set; }
    public int MaxHealth { get; set; }
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

public class MapCharacter : MapObject
{
    public string Race { get; set; }
    public string Class { get; set; }
    public int Strength { get; set; }
}

