using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameCode.Entities;

public class ItemInfoCard : Entity
{
    public Rectangle Bounds { get; set; }
    public Texture2D Texture { get; set; }

    public SpriteFont Font { get; set; }
    public string Name { get; set; }
    public string Info { get; set; }

    public Color TextColor { get; set; }
    public Color BackgroundColor { get; set; }
    public int Scale { get; set; } = 4;
    private Vector2 pos;
    public ItemInfoCard(BaseGame game, Vector2 pos, Texture2D texture, SpriteFont font, string name, string info) : base(game)
    {
        this.pos = pos;
        Texture = texture;
        Name = name;
        Info = info;
        TextColor = Color.White;
        BackgroundColor = Color.DarkGray;
        Font = font;

    }

    public override void Update(float dt)
    {
    }


    public override void Draw(SpriteBatch sb)
    {
        if (Texture == null) return;

        Bounds =
        new Rectangle(pos.ToPoint(), new Point(
            (Texture.Width * Scale) + 4,
            (Texture.Height * Scale) + ((Texture.Height * Scale) / 2)));

        sb.FillRectangle(Bounds, BackgroundColor, 0.7f);
        sb.Draw(Texture, new Rectangle(Bounds.X + 2, Bounds.Y + 2, Texture.Width * Scale, Texture.Height * Scale), null,
            Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.75f);

        var nameSize = Font.MeasureString(Name);
        sb.DrawString(Font,
            Name,
            new Vector2(Bounds.X + 2 + (Bounds.Width / 2) - (nameSize.X / 2), (Texture.Height * Scale) + 3), TextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.77f);

        var infoSize = Font.MeasureString(Info) * 0.75f;
        sb.DrawString(Font,
            Info,
            new Vector2(Bounds.X + 2 + (Bounds.Width / 2) - (infoSize.X / 2), (Texture.Height * Scale) + 3 + nameSize.Y + 3), TextColor, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.77f);
    }

}