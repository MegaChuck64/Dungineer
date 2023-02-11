using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Systems;

public class SpriteRenderSystem : BaseSystem
{
    private readonly Dictionary<string, Texture2D> textures;
    private readonly SpriteBatch sb; //todo
    public SpriteRenderSystem(BaseGame game, ContentManager content, params string[] textureNames) : base(game)
    {
        textures = new Dictionary<string, Texture2D>();

        var defaultTex = new Texture2D(game.GraphicsDevice, 1, 1);
        defaultTex.SetData(new Color[] { Color.White });
        textures.Add("_pixel", defaultTex);

        foreach (var name in textureNames)
        {
            textures.Add(name, ContentLoader.LoadTexture(name, content));
        }

        sb = new SpriteBatch(Game.GraphicsDevice);
    }
    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
     {
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.Opaque,
            samplerState: SamplerState.PointWrap,
            depthStencilState: DepthStencilState.Default,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            null); //camera here todo

        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<Transform>();
            if (transform == null || !transform.IsActive)
                continue;

            if (entity.GetComponent<Sprite>() is Sprite sprite && sprite.IsActive)
            {
                var bounds = transform.Bounds;
                sb.Draw(
                    textures[sprite.TextureName], 
                    bounds, 
                    sprite.Source, 
                    sprite.Tint, 
                    0f, 
                    Vector2.Zero, 
                    SpriteEffects.None, 
                    transform.Layer);
            }
        }

        sb.End();

    }
}