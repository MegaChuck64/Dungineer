using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;

public static class ContentLoader
{

    public static Texture2D TextureFromSpriteAtlas(
        string atlasName, Rectangle source, ContentManager content, GraphicsDevice graphicsDevice)
    {
        var atlas = LoadTexture(atlasName, content);

        var tex = new Texture2D(graphicsDevice, source.Width, source.Height);

        var data = new Color[source.Width * source.Height];
        
        atlas.GetData(0, source, data, 0, data.Length);
        
        tex.SetData(data);
        
        return tex;
    }

    public static Texture2D LoadTexture(string name, ContentManager content) =>
        content.Load<Texture2D>(@$"Tiles\{name}");

    public static SpriteFont LoadFont(string name, ContentManager content) =>
        content.Load<SpriteFont>(@$"Fonts\{name}");
}