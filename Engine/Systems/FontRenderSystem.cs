
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
            foreach (var text in entity.GetComponents<Text>().Where(t => t.IsActive))
            {
                var lines = text.Content.Split("\\n");
                var font = fonts[text.FontName];
                var lineHeight = font.MeasureString("M").Y;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var pos = transform.Position + text.Offset + new Vector2(0, lineHeight * i + 2);
                    sb.DrawString(font, line, pos, text.Tint);
                }
            }
        }

        sb.End();
    }

}