using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine;

public class Sprite : Entity
{
    public Transform2 Transform { get; set; }
    public Texture2D Texture { get; set; }
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
            Color.White);
    }
}