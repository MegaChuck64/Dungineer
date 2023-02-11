
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Systems;

public class FontRenderSystem : BaseSystem
{
    private readonly Dictionary<string, SpriteFont> fonts;
    private readonly SpriteBatch sb;
    public FontRenderSystem(BaseGame game, ContentManager content, params string[] fontNames) : base(game)
    {
        fonts = new Dictionary<string, SpriteFont>();
        foreach (var font in fontNames)
        {
            fonts.Add(font, ContentLoader.LoadFont(font, content));
        }
        sb = new SpriteBatch(Game.GraphicsDevice);
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        sb.Begin();

        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<Transform>();
            if (transform == null || !transform.IsActive)
                continue;

            if (entity.GetComponent<Text>() is Text text && text.IsActive)
            {
                var pos = transform.Position + text.Offset;

                sb.DrawString(fonts[text.FontName], text.Content, pos, text.Tint);
            }
        }

        sb.End();
    }

}