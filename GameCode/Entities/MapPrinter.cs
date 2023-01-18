
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace GameCode.Entities;

public class MapPrinter
{
    public Map Map { get; set; }
    public int TileSize { get; set; } = 32;
    public OrthographicCamera Camera { get; set; }

    public Vector2 MapToWorldPosition((int x, int y) tilePos) =>
    new(tilePos.x * TileSize, tilePos.y * TileSize);

    public (int x, int y) WorldToMapPosition(Vector2 worldPos) =>
        ((int)worldPos.X / TileSize, (int)worldPos.Y / TileSize);
    public Rectangle TileWorldBounds((int x, int y) tilePos) =>
        new(MapToWorldPosition(tilePos).ToPoint(), new Point(TileSize, TileSize));

    public bool IsOnScreen((int x, int y) tilePos) =>
      Camera.Contains(TileWorldBounds((tilePos.x, tilePos.y))) != ContainmentType.Disjoint;


    public Dictionary<Sprites, (string sprite, Rectangle source)> SpriteAtlas { get; set; }
    public Dictionary<string, Sprites> NameToSprite { get; set; }
    public Dictionary<Sprites, Texture2D> TextureAtlas { get; set; }

    public MapPrinter(ContentManager content)
    {
        SpriteAtlas = new Dictionary<Sprites, (string sprite, Rectangle source)>
        {
            { Sprites.Unknown , ("dg_grounds32", TileWorldBounds((0,0)))},
            { Sprites.Grass , ("dg_grounds32", TileWorldBounds((0,1)))},
            { Sprites.HumanFighter, ("HumanFighter", new Rectangle(11, 11, 32, 32))},
        };

        NameToSprite = new Dictionary<string, Sprites>
        {
            {"Unknown", Sprites.Unknown },
            {"Grass", Sprites.Grass },
            {"Human Fighter", Sprites.HumanFighter }
        };

        TextureAtlas = new Dictionary<Sprites, Texture2D>();
        foreach (var spr in SpriteAtlas)
        {
            TextureAtlas.Add(
                spr.Key, 
                Sprite.TextureFromSpriteAtlas(
                    spr.Value.sprite, 
                    spr.Value.source, 
                    content));
        }
    }

    public void Draw(SpriteBatch sb)
    {
        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                if (IsOnScreen((x, y)))
                {
                    DrawTile(sb, Map.Tiles[x, y]);

                    if (Map.GetMapObjects(x,y) is IEnumerable<MapObject> mapObjects)
                        foreach (var mo in mapObjects)
                        {
                            DrawTile(sb, mo);
                        }
                }
            }
        }
    }

    public void DrawTile(SpriteBatch sb, MapTile tile) =>
        sb.Draw(
            TextureAtlas[NameToSprite[tile.Name]], 
            TileWorldBounds((tile.X, tile.Y)), 
            Color.White);
    
}

public enum Sprites
{
    Unknown,
    Grass,
    HumanFighter
}