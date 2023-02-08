using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameCode.Entities;

public class Cursor : Component
{
    public Color Tint { get; set; } = Color.White;
    public float Radius { get; set; } = 4f;
    public Cursor(Entity entity) : base(entity)
    {
    }

    public override void Update(float dt)
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.DrawCircle(new CircleF(Owner.Game.MouseState.Position, Radius), 6, Tint, 1f, 1f);
    }
}