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
    private MouseState mouseState;
    private MouseState lastMouseState;

    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);
        sb = new SpriteBatch(game.GraphicsDevice);

        playerTexture = ContentLoader.LoadTexture("GnomeMage_32", content);
        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var mapEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Map));
        var map = mapEntity?.GetComponent<Map>();
        var mapTransform = mapEntity?.GetComponent<Transform>();

        var playerEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Player));
        var player = playerEntity?.GetComponent<Player>();
        var playerTransform = playerEntity?.GetComponent<Transform>();

        if (map == null || mapTransform == null || player == null || playerTransform == null)
            return;
        
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo

        lastMouseState = mouseState;
        mouseState = Mouse.GetState();
        var mouseTilePos = new Point(
                (int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
                (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);

       
        var path = 
            mouseTilePos.X >= 0 && 
            mouseTilePos.Y >= 0 && 
            mouseTilePos.X < map.GroundTiles.GetLength(0) &&
            mouseTilePos.Y < map.GroundTiles.GetLength(1) ? 
            
            GetPath( playerTransform.Position.ToPoint(), mouseTilePos, map) : 
            new List<Point>();


        if (path != null && path.Count > 0 && WasPressed)
        {
            playerTransform.Position = new Vector2(path.First().X, path.First().Y);
        }

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
                
                var tint = Color.White;

                if (path.Contains(new Point(x, y)))
                    tint = new Color(100, 100, 100);

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
                

                sb.Draw(txtr, bnds, tileInfo.Source, tint, 0f, Vector2.Zero, SpriteEffects.None, mapTransform.Layer);

  
            }
        }

        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            var objectTile = map.ObjectTiles[i];
            var objectTileInfo = Settings.TileAtlas[objectTile.Type];
            var tileBnds = new Rectangle(
              new Point(objectTile.X * Settings.TileSize, objectTile.Y * Settings.TileSize) +
              offset.ToPoint(),
              new Point(Settings.TileSize, Settings.TileSize));

            sb.Draw(
                objectTileInfo.Texture, 
                tileBnds, 
                objectTileInfo.Source, 
                objectTile.Tint, 
                0f, 
                Vector2.Zero, 
                SpriteEffects.None, 
                mapTransform.Layer + 0.05f);

        }



        var playerBnds = new Rectangle(
            new Point(playerTransform.Bounds.X * Settings.TileSize, playerTransform.Bounds.Y * Settings.TileSize) +
            offset.ToPoint(),
            new Point(Settings.TileSize, Settings.TileSize));

        sb.Draw(playerTexture, playerBnds, player.Source, player.Tint, 0f, Vector2.Zero, SpriteEffects.None, playerTransform.Layer);
        
        sb.End();
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