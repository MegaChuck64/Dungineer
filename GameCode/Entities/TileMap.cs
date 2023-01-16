using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace GameCode.Entities;

public class TileMap : Entity
{
    public Tile[,] Tiles { get; set; }
    public TileObject[,] TileObjects { get; set; }
    public Dictionary<TileType, Texture2D> TileTextures { get; set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int TileSize { get; private set; }
    public OrthographicCamera Camera { get; set; }
    public Vector2 MapToWorldPosition((int x, int y) tilePos) =>
        new(tilePos.x * TileSize, tilePos.y * TileSize);

    public (int x, int y) WorldToMapPosition(Vector2 worldPos) =>
        ((int)worldPos.X / TileSize, (int)worldPos.Y / TileSize);
    public Rectangle TileWorldBounds((int x, int y) tilePos) =>
        new(MapToWorldPosition(tilePos).ToPoint(), new Point(TileSize, TileSize));

    public bool IsOnScreen((int x, int y) tilePos) =>
        Camera.Contains(TileWorldBounds((tilePos.x, tilePos.y))) != ContainmentType.Disjoint;

    public Tile TryGetTile(int x, int y)
    {
        Tile tile = null;

        if (x >= 0 && x < Width &&
            y >= 0 && y < Height)
        {
            tile = Tiles[x, y];
        }

        return tile;
    }

    public TileObject TryGetTileObject(int x, int y )
    {
        TileObject tile = null;

        if (x >= 0 && x < Width &&
            y >= 0 && y < Height)
        {
            tile = TileObjects[x, y];
        }

        return tile;
    }
    public TileMap(BaseGame game, OrthographicCamera camera) : base(game)
    {
        Camera = camera;
        Width = 64;
        Height = 64;
        TileSize = 32;

        LoadTileTextures();

        Tiles = new Tile[Width, Height];
        TileObjects = new TileObject[Width, Height];
        var flip = false;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var typ = flip ? TileType.Grass : TileType.None;
                //var typ = PickTileType((x, y));
                Tiles[x, y] =
                    new Tile(
                        game,
                        typ,
                        TileTextures[typ],
                        new Vector2(x * TileSize, y * TileSize));

                if (x == 10 && y == 10)
                {
                    TileObjects[x, y] =
                        new Character(
                            Character.CharRace.Human,
                            Character.CharClass.Fighter,
                            game,
                            "Glenn",
                            TileType.HumanFighter,
                            TileTextures[TileType.HumanFighter],
                            new Vector2(x * TileSize, y * TileSize),
                            this);
                }
                else
                {
                    TileObjects[x, y] = null;
                }

                flip = !flip;
            }
            flip = !flip;
        }
    }

    static TileType PickTileType((int x, int y) tilePos) =>
        tilePos switch
        {
            { x: var X, y: var Y } when
            (X > 10 && Y > 10) => TileType.Grass,


            _ => TileType.None,
        };



    private void LoadTileTextures()
    {
        TileTextures = new Dictionary<TileType, Texture2D>
        {
            {
                TileType.None,
                Sprite.TextureFromSpriteAtlas("dg_grounds32", TileWorldBounds((0, 0)), Game.Content)
            },

            {
                TileType.Grass,
                Sprite.TextureFromSpriteAtlas("dg_grounds32", TileWorldBounds((0, 1)), Game.Content)
            },

            {
                TileType.HumanFighter,
                Sprite.TextureFromSpriteAtlas("HumanFighter", new Rectangle(11, 11, 32, 32), Game.Content)
            },
        };
    }


    public override void Update(float dt)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y].Update(dt);

                if (TileObjects[x, y] != null)
                    TileObjects[x, y].Update(dt);

            }
        }
    }
    public override void Draw(SpriteBatch sb)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (IsOnScreen((x, y)))
                {
                    Tiles[x, y].Draw(sb);

                    if (TileObjects[x, y] != null)
                        TileObjects[x, y].Draw(sb);
                }
            }
        }
    }
}

//adding new tile
//1. add it here
//2. load it in LoadTileTextures()
//3. use it in PickTileType((int x, int y) tilePos)
public enum TileType
{
    None,
    Grass,
    HumanFighter
}