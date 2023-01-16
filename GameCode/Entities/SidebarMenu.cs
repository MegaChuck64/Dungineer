using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Entities;

public class SidebarMenu : Entity
{
    public Rectangle Bounds { get; set; }
    public Texture2D Texture { get; set; }
    public Color Tint { get; set; }
    public SpriteFont Font { get; set; }
    public List<MenuItem> Items { get; set; }
    public SidebarMenu(BaseGame game, SpriteFont font) : base(game)
    {
        Font = font;
        Texture = Sprite.LoadTexture("hud-tileset", game.Content);
        Bounds = new Rectangle(10, 10, 300, 700);
        Tint = new Color(0, 0, 0, 170);
        Items = new List<MenuItem>();
    }  

    public void AddItem(string text, Color color, bool selectable = false)
    {
        Vector2 newPos;
        if (Items.Count > 0)
        {
            var lastPos = Items.Last().Position;
            newPos = new Vector2(lastPos.X, lastPos.Y + 40);
        }
        else
        {
            newPos = new Vector2(Bounds.X + 40, Bounds.Y + 40);
        }

        var newItem = new MenuItem(Game, text, newPos, color, Font)
        {
            IsSelectable = selectable
        };

        Items.Add(newItem);
    }

    public override void Update(float dt)
    {
        if (Game.KeyState.WasKeyJustDown(Keys.Tab))
        {

            int selected = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].IsSelected)
                {
                    Items[i].IsSelected = false;
                    selected = i + 1;
                    
                    if (selected >= Items.Count) selected = 0;

                    while (!Items[selected].IsSelectable && Items.Any(t=>t.IsSelectable))
                    {
                        selected++;
                        if (selected >= Items.Count)
                        {
                            selected = 0;
                        }
                    }                                       
                }
            }

            if (selected >= 0)
            {
                if (Items[selected].IsSelectable)
                    Items[selected].IsSelected = true;
            }

        }
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.FillRectangle(Bounds, Tint);
        foreach (var item in Items)
        {
            item.Draw(sb);
        }
    }
}

public class MenuItem : Entity
{
    public string Text { get; set; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public Color BGColor { get; set; }
    public SpriteFont Font { get; set; }
    public bool IsHeader { get; set; }
    public bool IsSelectable { get; set; }
    public bool IsSelected { get; set; }
    public MenuItem(BaseGame game, string text, Vector2 position, Color color, SpriteFont font, bool isHeader = false) : base(game)
    {
        Text = text;
        Position = position;
        Color = color;
        BGColor = new Color(0, 0, 60, 100);
        Font = font;
        IsHeader = isHeader;
    }
    public override void Update(float dt)
    {
    }
    public override void Draw(SpriteBatch sb)
    {
        var pos = Position;

        if (IsHeader)
            pos = new Vector2(pos.X - 10, pos.Y);

        var textSize = Font.MeasureString(Text);
        var bgTempColor = BGColor;

        if (IsSelected)
            bgTempColor = Color.CadetBlue;

        sb.FillRectangle(
            new Rectangle(
                (Position - new Vector2(2f, 2f)).ToPoint(), 
                (textSize + new Vector2(4f, 4f)).ToPoint()),
            bgTempColor);

        sb.DrawString(Font, Text, pos, Color);
    }
}