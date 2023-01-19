using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameCode.Entities;

public class Cursor : Entity
{
    public Color Tint { get; set; } = Color.White;
    public float Radius { get; set; } = 4f;
    public Cursor(BaseGame game) : base(game)
    {
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.DrawCircle(new CircleF(Game.MouseState.Position, Radius), 6, Tint, 1f, 1f);
    }
}