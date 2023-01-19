using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;

namespace Engine;

public class Sprite : Entity
{
    public Transform2 Transform { get; set; }
    public Texture2D Texture { get; set; }
    public float SortLayer { get; set; } = 0f;
    public Sprite(BaseGame game, Texture2D texture, Vector2 pos) : base(game)
    {
        Texture = texture;
        Transform = new Transform2(pos);
    }

    public override void Update(float dt)
    {

    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(
            Texture,
            new Rectangle(
                (int)Transform.Position.X,
                (int)Transform.Position.Y,
                (int)Transform.Scale.X * Texture.Width,
                (int)Transform.Scale.Y * Texture.Height),
            null,
            Color.White,
            0f,
            Vector2.Zero,           
            SpriteEffects.None,
            SortLayer);
    }

    public static Texture2D TextureFromSpriteAtlas(
        string atlasName, Rectangle source, ContentManager content)
    {
        var atlas = content.Load<Texture2D>($@"Tiles\{atlasName}");

        var tex = new Texture2D(content.GetGraphicsDevice(), source.Width, source.Height);

        var data = new Color[source.Width * source.Height];
        
        atlas.GetData(0, source, data, 0, data.Length);
        
        tex.SetData(data);
        
        return tex;
    }

    public static Texture2D LoadTexture(string name, ContentManager content) =>
        content.Load<Texture2D>(@$"Sprites\{name}");

}