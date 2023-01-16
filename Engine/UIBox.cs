using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine;

public class UIBox : Entity
{
    public Rectangle Bounds { get; set; }
    public Texture2D Texture { get; set; }
    public Color Tint { get; set; } = Color.White;
    public string Data { get; set; }    
    public SpriteFont Font { get; set; }
    public UIBox(BaseGame game, Texture2D texture, Rectangle bounds, SpriteFont font) : base(game)
    {
        Texture = texture;
        Bounds = bounds;
        Font = font;
    }
    public override void Update(float dt)
    {
    }
    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(Texture, Bounds, Tint);

        sb.DrawString(Font, Data, Bounds.Location.ToVector2() + new Vector2(200, 100), Color.Yellow);
    }
}