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

                sb.Draw(txtr, bnds, tileInfo.Source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, mapTransform.Layer);

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

}