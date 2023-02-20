using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class MapSystem : BaseSystem
{
    private SpriteBatch sb;
    private Vector2 offset;
    private Texture2D tileSelectTexture;
    private MouseState mouseState;
    private MouseState lastMouseState;

    private const float groundLayer = 0.5f;
    private const float objectLayer = 0.6f;
    private const float effectLayer = 0.7f;
    private const float itemLayer = 0.8f;


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

    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);
        sb = new SpriteBatch(game.GraphicsDevice);

        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

        var map = entities.First(t => t.Components.Any(v => v is Map)).GetComponent<Map>();

        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        foreach (var ent in entities)
        {
            if (ent.HasTag("Cursor"))
            {
                var cursorSprite = ent.GetComponent<Sprite>();

                cursorSprite.IsActive = MapPixelBounds.Contains(mouseState.Position) == false;
            }
            else if (ent.HasTag("Player"))
            {
                var playerObject = ent.GetComponent<MapObject>();
                if (WasPressed && MapPixelBounds.Contains(mouseState.Position))
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
                if (WasPressed && MapPixelBounds.Contains(mouseState.Position))
                {

                    MoveGhost(ghostObj, player, map, entities
                            .Where(g => g.Components.Any(h => h is MapObject))
                            .Select(n => n.GetComponent<MapObject>())
                            .ToArray());
                }
            }
        }
    }

    //items that will move around our map
    private void DrawMapObject(MapObject mapObject)
    {
        var mapObjectInfo = Settings.MapObjectAtlas[mapObject.Type];
        var texture = Settings.TextureAtlas[mapObjectInfo.TextureName];

        var bnds = GetTileBounds(mapObject.MapX, mapObject.MapY);

        sb.Draw(
            texture,
            bnds,
            mapObjectInfo.Source,
            mapObject.Tint,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            itemLayer);


    }
    private void DrawTile(Tile tile, float layer)
    {
        var tileInfo = Settings.TileAtlas[tile.Type];
        var texture = Settings.TextureAtlas[tileInfo.TextureName];

        var bnds = GetTileBounds(tile.X, tile.Y);

        sb.Draw(texture, bnds, tileInfo.Source, tile.Tint, 0f, Vector2.Zero, SpriteEffects.None, layer);
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

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

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
            else if (ent.HasTag("Cursor"))
            {
                var cursorTransform = ent.GetComponent<Transform>();
                var cursorSprite = ent.GetComponent<Sprite>();

                if (!cursorSprite.IsActive)
                {
                    sb.Draw(
                        tileSelectTexture,
                        GetTileBounds(MouseTilePosition),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        effectLayer);
                }

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

                        sb.Draw(wtxt, bnds, winfo.Source, mapObj.Tint, 0f, Vector2.Zero, SpriteEffects.None, itemLayer + 0.1f);
                    }
                }
            }
        }

        sb.End();

    }


    ////get map data
    //var mapEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Map));
    //var map = mapEntity?.GetComponent<Map>();
    //var mapTransform = mapEntity?.GetComponent<Transform>();



    //if (map == null || mapTransform == null || player == null || playerTransform == null)
    //    return;

    ////get mouse data
    //lastMouseState = mouseState;
    //mouseState = Mouse.GetState();
    //var mouseTilePos = new Point(
    //        (int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
    //        (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);

    ////get path between player and mouse
    //var path =
    //    mouseTilePos.X >= 0 &&
    //    mouseTilePos.Y >= 0 &&
    //    mouseTilePos.X < map.GroundTiles.GetLength(0) &&
    //    mouseTilePos.Y < map.GroundTiles.GetLength(1) ?

    //    GetPath(playerTransform.Position.ToPoint(), mouseTilePos, map) :
    //    new List<Point>();

    ////if clicked on a tile that we can travel to move player one step towards it
    //if (path != null && path.Count > 0 && WasPressed)
    //{
    //    playerTransform.Position = new Vector2(path.First().X, path.First().Y);
    //}


    ////DRAWING
    //sb.Begin(
    //    sortMode: SpriteSortMode.FrontToBack,
    //    blendState: BlendState.NonPremultiplied,
    //    samplerState: SamplerState.PointClamp,
    //    depthStencilState: DepthStencilState.DepthRead,
    //    rasterizerState: RasterizerState.CullCounterClockwise,
    //    effect: null,
    //    transformMatrix: null); //camera here todo


    ////draw ground tiles
    //for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
    //{
    //    for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
    //    {
    //        var groundTile = map.GroundTiles[x, y];
    //        var tileInfo = Settings.TileAtlas[groundTile.Type];

    //        var txtr = tileInfo.Texture;

    //        var bnds = new Rectangle(
    //            mapTransform.Bounds.Location + offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
    //            new Point(Settings.TileSize, Settings.TileSize));




    //        //draw ground tile
    //        sb.Draw(txtr, bnds, tileInfo.Source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, mapTransform.Layer);

    //        //draw path semi transparent path texture over tiles that are part of our player path
    //        if (path.Contains(new Point(x, y)))
    //            sb.Draw(
    //                pathTexture,
    //                bnds,
    //                new Rectangle(0, 0, Settings.TileSize, Settings.TileSize),
    //                new Color(100, 200, 255, 100),
    //                0f,
    //                Vector2.Zero,
    //                SpriteEffects.None,
    //                mapTransform.Layer + 0.1f);

    //        //if mouse is over tile, draw tile selector over it
    //        if (bnds.Contains(Mouse.GetState().Position))
    //        {
    //            sb.Draw(
    //                tileSelectTexture,
    //                bnds,
    //                null,
    //                Color.White,
    //                0f,
    //                Vector2.Zero,
    //                SpriteEffects.None,
    //                0.75f);
    //        }
    //    }
    //}

    ////draw object tiles 
    //for (int i = 0; i < map.ObjectTiles.Count; i++)
    //{
    //    var objectTile = map.ObjectTiles[i];
    //    var objectTileInfo = Settings.TileAtlas[objectTile.Type];

    //    var tileBnds = new Rectangle(
    //      new Point(objectTile.X * Settings.TileSize, objectTile.Y * Settings.TileSize) +
    //      offset.ToPoint(),
    //      new Point(Settings.TileSize, Settings.TileSize));

    //    //draw object tile
    //    sb.Draw(
    //        objectTileInfo.Texture, 
    //        tileBnds, 
    //        objectTileInfo.Source, 
    //        objectTile.Tint, 
    //        0f, 
    //        Vector2.Zero, 
    //        SpriteEffects.None, 
    //        mapTransform.Layer + 0.05f);

    //    //if (WasPressed)
    //    //{
    //    //    //move ghost on tick
    //    //    if (objectTile.Type == TileType.Ghost)
    //    //    {
    //    //        MoveGhost(objectTile, playerTransform.Position.ToPoint(), map);
    //    //    }
    //    //}

    //}


    ////Draw player
    //var playerBnds = new Rectangle(
    //    new Point(playerTransform.Bounds.X * Settings.TileSize, playerTransform.Bounds.Y * Settings.TileSize) +
    //    offset.ToPoint(),
    //    new Point(Settings.TileSize, Settings.TileSize));

    //sb.Draw(playerTexture, playerBnds, player.Source, player.Tint, 0f, Vector2.Zero, SpriteEffects.None, playerTransform.Layer);

    //sb.End();
    //  }

    private void SwapObjectTiles(Tile objA, Tile objB)
    {
        var tempX = objA.X;
        var tempY = objA.Y;

        objA.X = objB.X;
        objA.Y = objB.Y;

        objB.X = tempX;
        objB.Y = tempY;
    }
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

        //if clicked on a tile that we can travel to move player one step towards it
        if (path != null && path.Count > 0)
        {
            var nextStep = path.First();
            player.MapX = nextStep.X;
            player.MapY = nextStep.Y;
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

    private bool WasPressed => lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;

}