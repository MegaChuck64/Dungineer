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
    private Texture2D playerTexture;
    private Texture2D tileSelectTexture;
    private Texture2D pathTexture; 
    private MouseState mouseState;
    private MouseState lastMouseState;

    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);
        sb = new SpriteBatch(game.GraphicsDevice);

        playerTexture = ContentLoader.LoadTexture("GnomeMage_32", content);
        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
        pathTexture = ContentLoader.LoadTexture("effects_32", content);

    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //get map data
        var mapEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Map));
        var map = mapEntity?.GetComponent<Map>();
        var mapTransform = mapEntity?.GetComponent<Transform>();

        //get player data
        var playerEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Player));
        var player = playerEntity?.GetComponent<Player>();
        var playerTransform = playerEntity?.GetComponent<Transform>();

        
        if (map == null || mapTransform == null || player == null || playerTransform == null)
            return;

        //get mouse data
        lastMouseState = mouseState;
        mouseState = Mouse.GetState();
        var mouseTilePos = new Point(
                (int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
                (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);

        //get path between player and mouse
        var path =
            mouseTilePos.X >= 0 &&
            mouseTilePos.Y >= 0 &&
            mouseTilePos.X < map.GroundTiles.GetLength(0) &&
            mouseTilePos.Y < map.GroundTiles.GetLength(1) ?

            GetPath(playerTransform.Position.ToPoint(), mouseTilePos, map) :
            new List<Point>();

        //if clicked on a tile that we can travel to move player one step towards it
        if (path != null && path.Count > 0 && WasPressed)
        {
            playerTransform.Position = new Vector2(path.First().X, path.First().Y);
        }


        //DRAWING
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo


        //draw ground tiles
        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var groundTile = map.GroundTiles[x, y];
                var tileInfo = Settings.TileAtlas[groundTile.Type];

                var txtr = tileInfo.Texture;
                    
                var bnds = new Rectangle(
                    mapTransform.Bounds.Location + offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
                    new Point(Settings.TileSize, Settings.TileSize));
                



                //draw ground tile
                sb.Draw(txtr, bnds, tileInfo.Source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, mapTransform.Layer);

                //draw path semi transparent path texture over tiles that are part of our player path
                if (path.Contains(new Point(x, y)))
                    sb.Draw(
                        pathTexture,
                        bnds,
                        new Rectangle(0, 0, Settings.TileSize, Settings.TileSize),
                        new Color(100, 200, 255, 100),
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        mapTransform.Layer + 0.1f);

                //if mouse is over tile, draw tile selector over it
                if (bnds.Contains(Mouse.GetState().Position))
                {
                    sb.Draw(
                        tileSelectTexture,
                        bnds,
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.75f);
                }
            }
        }

        //draw object tiles 
        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            var objectTile = map.ObjectTiles[i];
            var objectTileInfo = Settings.TileAtlas[objectTile.Type];

            var tileBnds = new Rectangle(
              new Point(objectTile.X * Settings.TileSize, objectTile.Y * Settings.TileSize) +
              offset.ToPoint(),
              new Point(Settings.TileSize, Settings.TileSize));

            //draw object tile
            sb.Draw(
                objectTileInfo.Texture, 
                tileBnds, 
                objectTileInfo.Source, 
                objectTile.Tint, 
                0f, 
                Vector2.Zero, 
                SpriteEffects.None, 
                mapTransform.Layer + 0.05f);

            if (WasPressed)
            {
                //move ghost on tick
                if (objectTile.Type == TileType.Ghost)
                {
                    MoveGhost(objectTile, playerTransform.Position.ToPoint(), map);
                }
            }

        }



        var playerBnds = new Rectangle(
            new Point(playerTransform.Bounds.X * Settings.TileSize, playerTransform.Bounds.Y * Settings.TileSize) +
            offset.ToPoint(),
            new Point(Settings.TileSize, Settings.TileSize));

        sb.Draw(playerTexture, playerBnds, player.Source, player.Tint, 0f, Vector2.Zero, SpriteEffects.None, playerTransform.Layer);
        
        sb.End();
    }

    private void SwapObjectTiles(Tile objA, Tile objB)
    {
        var tempX = objA.X;
        var tempY = objA.Y;

        objA.X = objB.X;
        objA.Y = objB.Y;

        objB.X = tempX;
        objB.Y = tempY;
    }
    private void MoveGhost(Tile ghost, Point target, Map map)
    {
        if (Vector2.Distance(new Vector2(ghost.X, ghost.Y), target.ToVector2()) < 6)
        {
            if (GetPath(new Point(ghost.X, ghost.Y), target, map)?.FirstOrDefault() is Point nextPos)
            {
                if (nextPos != target)
                {
                    ghost.X = nextPos.X;
                    ghost.Y = nextPos.Y;
                }
                else
                {
                    //attack
                }
            }
        }
    }

    public List<Point> GetPath(Point start, Point end, Map map)
    {

        var grid = new bool[map.GroundTiles.GetLength(0), map.GroundTiles.GetLength(1)];


        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var groundTile = Settings.TileAtlas[map.GroundTiles[x, y].Type];
                var objectTiles = map.ObjectTiles.Where(t => t.X == x && t.Y == y).Select(v => Settings.TileAtlas[v.Type]);
                grid[x, y] = !groundTile.Solid && !objectTiles.Any(y => y.Solid);
            }

        }

        var pathFinder = new PathFinder(new PathFinderSearchParams(start, end, grid));

        return pathFinder.FindPath();

    }

    private bool WasPressed => lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;

}