using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameCode.Entities;

public class CharacterInfoCard : Entity
{
    public Rectangle Bounds { get; set; }
    public Texture2D Texture { get; set; }

    public SpriteFont NameFont { get; set; }
    public SpriteFont InfoFont { get; set; }
    public string Name { get; set; }
    public string Info { get; set; }

    public Color TextColor { get; set; }
    public Color BackgroundColor { get; set; }
    public int Scale { get; set; } = 4;
    public CharacterInfoCard(BaseGame game, Rectangle bounds, Texture2D texture, SpriteFont nameFont, SpriteFont infoFont, string name, string info) : base(game)
    {
        Bounds = bounds;
        Texture = texture;
        Name = name;
        Info = info;
        TextColor = Color.White;
        BackgroundColor = Color.DarkGray;
        NameFont = nameFont;
        InfoFont = infoFont;
    }

    public override void Update(float dt)
    {
    }


    public override void Draw(SpriteBatch sb)
    {
        if (Texture == null) return;

        sb.FillRectangle(Bounds, BackgroundColor, 0.7f);

        //figure out height;

        sb.Draw(
            Texture, 
            new Rectangle(Bounds.X + 1, Bounds.Y + 1, Bounds.Width - 2, Bounds.Width - 2), 
            null,
            Color.White, 
            0f, 
            Vector2.Zero, 
            SpriteEffects.None, 
            0.75f);

        var nameSize = NameFont.MeasureString(Name);
        sb.DrawString(
            NameFont,
            Name,
            new Vector2(Bounds.X + 2 + (Bounds.Width / 2) - (nameSize.X / 2), Bounds.Y + Bounds.Width + 2),
            TextColor,
            0f, 
            Vector2.Zero, 
            1f, 
            SpriteEffects.None, 
            0.77f);

        var infoSize = InfoFont.MeasureString(Info);
        sb.DrawString(
            InfoFont,
            Info,
            new Vector2(
                Bounds.X + 2 + (Bounds.Width / 2) - (infoSize.X / 2), 
                Bounds.Y + Bounds.Width + 2 + nameSize.Y + 2), 
            TextColor, 
            0f, 
            Vector2.Zero, 
            1f,
            SpriteEffects.None, 
            0.77f);
    }

}