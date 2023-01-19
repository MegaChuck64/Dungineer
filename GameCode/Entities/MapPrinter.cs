
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace GameCode.Entities;

public class MapPrinter : Entity
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

    public MapPrinter(BaseGame game, int fighterSpriteIndex = 0) : base(game)
    {
        SpriteAtlas = new Dictionary<Sprites, (string sprite, Rectangle source)>
        {
            { Sprites.Unknown , ("grounds_32", TileWorldBounds((0,0)))},
            { Sprites.Grass , ("grounds_32", TileWorldBounds((0,1)))},
            { Sprites.HumanFighter, ("HumanFighter_32", TileWorldBounds((fighterSpriteIndex,0)))},
            { Sprites.PineTree, ("trees_32", TileWorldBounds((3,0)))},
            { Sprites.TileSelector, ("ui_box_select_32", TileWorldBounds((0,0))) }
        };

        NameToSprite = new Dictionary<string, Sprites>
        {
            {"Unknown", Sprites.Unknown },
            {"Grass", Sprites.Grass },
            {"Human Fighter", Sprites.HumanFighter },
            {"Pine Tree", Sprites.PineTree },
            {"Tile Selector", Sprites.TileSelector },
        };

        TextureAtlas = new Dictionary<Sprites, Texture2D>();
        foreach (var spr in SpriteAtlas)
        {
            TextureAtlas.Add(
                spr.Key, 
                Sprite.TextureFromSpriteAtlas(
                    spr.Value.sprite, 
                    spr.Value.source, 
                    game.Content));
        }
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
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
    HumanFighter,
    PineTree,
    TileSelector,
}