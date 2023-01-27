using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Entities;

public class TileMap : Entity
{
    public GroundTile[,] Ground { get; private set; }
    public List<TileObject> TileObjects { get; set; }
    public FastRandom Rand { get; set; }

    public int TileSize { get; set; } = 32;
    public int Width { get; set; }
    public int Height { get; set; }
    public OrthographicCamera Camera { get; set; }
    public TileMap(BaseGame game, int width, int height, FastRandom rand, OrthographicCamera cam) : base(game)
    {
        Rand = rand;
        Width = width;
        Height = height;
        Camera = cam;
        Create();
    }

    public void Create()
    {

        TileObjects?.Clear();
        TileObjects ??= new List<TileObject>();
        var treeChance = 0.25f;
        Ground = new GroundTile[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var ground = TileLoader.GetTileObject(t => t is GroundTile) as GroundTile;
                ground.X = x;
                ground.Y = y;
                Ground[x, y] = ground;

                if (Rand.NextSingle(0f, 1f) <= treeChance)
                {
                    var tree = TileLoader.GetTileObject(t => t.Name.EndsWith("Tree")) as ItemTile;
                    tree.X = x;
                    tree.Y = y;
                    TileObjects.Add(tree);
                }
            }
        }
    }

    public GroundTile GetGroundTile(int x, int y) =>
        (x >= 0 && x < Width && y >= 0 && y < Height)
        ? Ground[x, y] : null;

    public IEnumerable<TileObject> GetMapObjects(int x, int y) =>
        TileObjects.Where(o =>
        o.X == x &&
        o.Y == y);

    public IEnumerable<TileObject> GetObjectsInRadius(int x, int y, int r) =>
        TileObjects.Where(
            o => Vector2.Distance(
                new Vector2(x, y),
                new Vector2(o.X, o.Y))
            <= r);

    public IEnumerable<GroundTile> GetAdjacentTiles(int x, int y, bool includeDiagonals)
    {
        var tiles = new List<GroundTile>();

        if (x > 0) tiles.Add(GetGroundTile(x - 1, y));
        if (x < Width - 1) tiles.Add(GetGroundTile(x + 1, y));
        if (y > 0) tiles.Add(GetGroundTile(x, y - 1));
        if (y < Height - 1) tiles.Add(GetGroundTile(x, y + 1));

        if (includeDiagonals)
        {
            if (x > 0 && y > 0) tiles.Add(GetGroundTile(x - 1, y - 1));
            if (x < Width - 1 && y < Width - 1) tiles.Add(GetGroundTile(x + 1, y + 1));
            if (x > 0 && y < Width - 1) tiles.Add(GetGroundTile(x - 1, y + 1));
            if (x < Width - 1 && y >= 0) tiles.Add(GetGroundTile(x + 1, y - 1));
        }

        return tiles;
    }

    public Vector2 MapToWorldPosition((int x, int y) tilePos) =>
   new(tilePos.x * TileSize, tilePos.y * TileSize);

    public (int x, int y) WorldToMapPosition(Point worldPos) =>
        (worldPos.X / TileSize, worldPos.Y / TileSize);
    public Rectangle TileWorldBounds((int x, int y) tilePos) =>
        new(MapToWorldPosition(tilePos).ToPoint(), new Point(TileSize, TileSize));

    public bool IsOnScreen((int x, int y) tilePos) =>
      Camera.Contains(TileWorldBounds((tilePos.x, tilePos.y))) != ContainmentType.Disjoint;


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
                        (Ground[x, y].Flags?.Any(t => t == "Solid") ?? false ||
                        GetMapObjects(x, y).Any(t => t.Flags.Contains("Solid"))) ?
                        (short)0 : (short)1;
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
    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y= 0; y < Height; y++)
            {
                sb.Draw(
                    Ground[x, y].Sprite, 
                    new Rectangle(x * 32, y * 32, 32, 32), 
                    null, 
                    Color.White,
                    0f, 
                    Vector2.Zero, 
                    SpriteEffects.None, 
                    0.1f);
            }
        }

        for (int i = 0; i < TileObjects.Count; i++)
        {
            sb.Draw(
                TileObjects[i].Sprite, 
                new Rectangle(TileObjects[i].X * 32, TileObjects[i].Y * 32, 32, 32), 
                null, 
                Color.White,
                0f,
                Vector2.Zero, 
                SpriteEffects.None,
                0.2f);
        }
    }
}