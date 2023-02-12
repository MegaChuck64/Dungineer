using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class MapSystem : BaseSystem
{
    private Dictionary<int, MapItem> mapItems;
    
    private SpriteBatch sb;
    private Vector2 offset;
    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);
        sb = new SpriteBatch(game.GraphicsDevice);
        mapItems = new Dictionary<int, MapItem>();
        var mapVals = ContentLoader.LoadText("MapValues.txt", content);
        foreach (var val in mapVals)
        {
            var splt = val.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            var ndx = int.Parse(splt[0]);
            var txtrName = splt[1];
            var x = int.Parse(splt[2]);
            var y = int.Parse(splt[3]);
            var w = int.Parse(splt[4]);
            var h = int.Parse(splt[5]);
            var txtr = ContentLoader.LoadTexture(txtrName, content);
            mapItems.Add(ndx, new MapItem { source = new Rectangle(x, y, w, h), texture = txtr});
        }
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var mapEntity = entities.FirstOrDefault(t => t.Components.Any(v => v is Map));
        var map = mapEntity?.GetComponent<Map>();
        if (map == null)
            return;
        var transform = mapEntity?.GetComponent<Transform>();
        if (transform == null)
            return;

        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo

        for (int x = 0; x < map.Tiles.GetLength(0); x++)
        {
            for (int y =0; y < map.Tiles.GetLength(1); y++)
            {
                var mapItem = mapItems[map.Tiles[x, y]];
                var txtr = mapItem.texture;
                var bnds = new Rectangle(transform.Bounds.Location + offset.ToPoint() + new Point(x * (int)transform.Size.X, y * (int)transform.Size.Y), transform.Bounds.Size);
                sb.Draw(txtr, bnds, mapItem.source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, transform.Layer);
            }
        }

        sb.End();
    }


    private struct MapItem
    {
        public Texture2D texture;
        public Rectangle source;
    }
}