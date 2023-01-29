using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;

namespace GameCode.Entities;

public class Terminal : Entity
{
    public List<string> Lines { get; set; }
    public Rectangle Bounds { get; set; }
    public SpriteFont Font { get; set; }
    public bool Active { get; set; } = false;
    public Color TextColor { get; set; } = Color.Yellow;
    public Color BackgroundColor { get; set; } = Color.DarkBlue;
    public string Input { get; set; }

    private bool showCursor = false;
    private float cursorBlinkSpeed = 2f;
    private float cursorBlinkTimer = 0f;
    public Terminal(BaseGame game, SpriteFont font) : base(game)
    {
        Font = font;
        Lines = new List<string>();
        Input = string.Empty;
        Game.Window.TextInput += Window_TextInput;
    }

    private void Window_TextInput(object sender, TextInputEventArgs e)
    {
        if (!Active) return;

        if (e.Key == Keys.Back)
        { 
            if (Input.Length > 0)
                Input = Input[..^1];
        }
        else
            Input += e.Character;
    }

    public override void Update(float dt)
    {
        if (!Active) return;

        if (Game.KeyState.WasKeyJustDown(Keys.Enter))
        {
            if (!string.IsNullOrWhiteSpace(Input))
            {
                SubmitInput();
            }
        }

        cursorBlinkTimer += dt;
        if (cursorBlinkTimer >= 1f/cursorBlinkSpeed)
        {
            showCursor = !showCursor;
            cursorBlinkTimer = 0f;
        }
    }

    public void SubmitInput()
    {
        Lines.Add(Input);
        Input = string.Empty;
    }

    public override void Draw(SpriteBatch sb)
    {
        var letterSize = Font.MeasureString("M");
        
        for (int i = 0; i < Lines.Count; i++)
        {
            var line = Lines[Lines.Count - i - 1];
            var pos = new Vector2(Bounds.X + 4, Bounds.Bottom - ((letterSize.Y + 2) * (i + 2)));
            if (Bounds.Contains(pos))
                sb.DrawString(Font, "#>" + line, pos, TextColor);
        }

        sb.FillRectangle(
            new Rectangle(
                Bounds.X, 
                Bounds.Bottom - 4 - (int)letterSize.Y - 2, 
                Bounds.Width, 
                (int)letterSize.Y + 4), 
            new Color(22, 22, 22), 0.5f);

        sb.DrawString(
            Font, 
            "?>" + Input + (showCursor ? "|" : ""), 
            new Vector2(Bounds.X + 4, Bounds.Bottom - letterSize.Y), 
            TextColor, 
            0f, 
            Vector2.Zero, 
            1f, 
            SpriteEffects.None, 
            0.6f);

        sb.DrawRectangle(Bounds, TextColor, 1, 0.55f);
    }
}