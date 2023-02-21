using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class MapSystem : BaseSystem
{
    private Vector2 offset;

    private MouseState mouseState;
    private MouseState lastMouseState;

    private const float groundLayer = 0.5f;
    private const float objectLayer = 0.6f;
    private const float effectLayer = 0.7f;
    private const float itemLayer = 0.8f;

    private readonly Texture2D tileSelectTexture;
    private readonly SpriteBatch sb;


    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);

        sb = new SpriteBatch(game.GraphicsDevice);

        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

        var map = entities.FirstOrDefault(t => t.Components.Any(v => v is Map))?.GetComponent<Map>();

        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        foreach (var ent in entities)
        {
            if (ent.HasTag("Player"))
            {
                var playerObject = ent.GetComponent<MapObject>();
                if (MouseWasClicked && MapPixelBounds.Contains(mouseState.Position))
                {
                    MovePlayer(playerObject, map, entities
                                .Where(g => g.Components.Any(h => h is MapObject))
                                .Select(n => n.GetComponent<MapObject>())
                                .ToArray());
                }
            }
            else if (ent.HasTag("Ghost"))
            {
                var player = entities.First(t => t.HasTag("Player")).GetComponent<MapObject>();
                var ghostObj = ent.GetComponent<MapObject>();
                if (MouseWasClicked && MapPixelBounds.Contains(mouseState.Position))
                {

                    MoveGhost(ghostObj, player, map, entities
                            .Where(g => g.Components.Any(h => h is MapObject))
                            .Select(n => n.GetComponent<MapObject>())
                            .ToArray());
                }
            }
            else if (ent.HasTag("Cursor"))
            {
                ent.GetComponent<UIElement>().IsActive = !MapPixelBounds.Contains(mouseState.Position);
            }
        }
    }



    
    #region Drawing

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //if (SceneManager.CurrentScene != "Play") return;

        //DRAWING
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo





        foreach (var ent in entities)
        {
            if (ent.HasTag("Map"))
            {
                DrawMap(ent);
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {
                DrawMapObject(mapObj);
                if (ent.GetComponent<Wardrobe>() is Wardrobe wardrobe)
                {
                    if (wardrobe.BodySlot.HasValue)
                    {
                        var winfo = Settings.WardrobeAtlas[wardrobe.BodySlot.Value];
                        var wtxt = Settings.TextureAtlas[winfo.TextureName];

                        var bnds = GetTileBounds(mapObj.MapX, mapObj.MapY);
                        bnds = new Rectangle(bnds.X, bnds.Y, (int)(bnds.Width * mapObj.Scale), (int)(bnds.Height * mapObj.Scale));
                        sb.Draw(
                            wtxt, 
                            bnds, 
                            winfo.Source, 
                            mapObj.Tint, 
                            0f, 
                            mapObj.Scale == 1f ? Vector2.Zero : new Vector2(bnds.Width/2, bnds.Height/2), 
                            SpriteEffects.None, 
                            itemLayer + 0.1f);
                    }
                }
            }
        }

        sb.End();

    }


    private void DrawMap(Entity ent)
    {
        var map = ent.GetComponent<Map>();
        if (map == null) throw new System.Exception("Entity tagged with 'Map' must have map component");


        //ground tiles
        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                DrawTile(map.GroundTiles[x, y], groundLayer);
            }
        }

        //object tiles
        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            DrawTile(map.ObjectTiles[i], objectLayer);
        }
    }

    //items that will move around our map
    private void DrawMapObject(MapObject mapObject)
    {
        var mapObjectInfo = Settings.MapObjectAtlas[mapObject.Type];
        var texture = Settings.TextureAtlas[mapObjectInfo.TextureName];

        var bnds = GetTileBounds(mapObject.MapX, mapObject.MapY);
        bnds = new Rectangle(bnds.X, bnds.Y, (int)(bnds.Width * mapObject.Scale), (int)(bnds.Height * mapObject.Scale));

        sb.Draw(
            texture,
            bnds,
            mapObjectInfo.Source,
            mapObject.Tint,
            0f,
            mapObject.Scale == 1f ? Vector2.Zero : new Vector2(bnds.Width / 2, bnds.Height / 2),
            SpriteEffects.None,
            itemLayer);


    }
    private void DrawTile(Tile tile, float layer)
    {
        var tileInfo = Settings.TileAtlas[tile.Type];
        var texture = Settings.TextureAtlas[tileInfo.TextureName];

        var bnds = GetTileBounds(tile.X, tile.Y);

        sb.Draw(texture, bnds, tileInfo.Source, tile.Tint, 0f, Vector2.Zero, SpriteEffects.None, layer);

        if (bnds.Contains(mouseState.Position))
        {
            sb.Draw(
                tileSelectTexture,
                bnds,
                new Rectangle(0, 0, 32, 32),
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                layer + 0.1f);
        }

    }

    #endregion

    
    #region Creature Movement

    private void MoveGhost(MapObject ghost, MapObject target, Map map, params MapObject[] mapObjects)
    {
        if (Vector2.Distance(new Vector2(ghost.MapX, ghost.MapY), new Vector2(target.MapX, target.MapY)) < 6)
        {
            var targAdj = map.GetAdjacentEmptyTiles(target.MapX, target.MapY, true, mapObjects);

            if (targAdj.Any())
            {
                var targ = targAdj.First();
                var path = GetPath(
                new Point(ghost.MapX, ghost.MapY),
                new Point(targ.x, targ.y),
                    map,
                    mapObjects);

                if (path != null && path.Count > 0)
                {
                    var nextStep = path.First();

                    if (nextStep != new Point(target.MapX, target.MapY))
                    {
                        ghost.MapX = nextStep.X;
                        ghost.MapY = nextStep.Y;
                    }
                    else
                    {
                        //attack
                    }
                }
            }

        }
    }

    private void MovePlayer(MapObject player, Map map, params MapObject[] mapObjects)
    {
        var path = GetPath(
            new Point(player.MapX, player.MapY),
                MouseTilePosition,
                map,
                mapObjects);

        if (path != null && path.Count > 0)
        {
            var nextStep = path.First();
            player.MapX = nextStep.X;
            player.MapY = nextStep.Y;
        }
    }

    #endregion


    #region Helpers 

    private bool MouseWasClicked => lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;

    private Rectangle MapPixelBounds => new(Game.Width / 5, 0, (Game.Width / 5) * 3, Game.Height);

    private Rectangle GetTileBounds(Point pos) => GetTileBounds(pos.X, pos.Y);

    private Rectangle GetTileBounds(int x, int y) =>
        new Rectangle(
            offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
            new Point(Settings.TileSize, Settings.TileSize));


    private Point MouseTilePosition
    {
        get
        {
            return new Point(
                (int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
                (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);
        }
    }


    public List<Point> GetPath(Point start, Point end, Map map, params MapObject[] mapObjects)
    {

        var grid = new bool[map.GroundTiles.GetLength(0), map.GroundTiles.GetLength(1)];


        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var groundTile = Settings.TileAtlas[map.GroundTiles[x, y].Type];
                var objectTiles = map.ObjectTiles.Where(t => t.X == x && t.Y == y).Select(v => Settings.TileAtlas[v.Type]);

                var hasGroundCollision = groundTile.Solid;
                var hasObjectCollision = objectTiles.Any(y => y.Solid);
                var hasItemCollision = mapObjects.Any(g => g.MapX == x && g.MapY == y);

                grid[x, y] = !hasGroundCollision && !hasObjectCollision && !hasItemCollision;
            }

        }

        var pathFinder = new PathFinder(new PathFinderSearchParams(start, end, grid));

        return pathFinder.FindPath();

    }
    #endregion
}