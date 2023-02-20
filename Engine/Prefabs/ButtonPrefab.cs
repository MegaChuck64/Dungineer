using Engine.Components;
using Microsoft.Xna.Framework;

namespace Engine.Prefabs;
public class ButtonPrefab : IPrefab<Entity>
{
    private Color hoverColor;
    private Color pressedColor;
    private Color defaultColor;
    private Color txtColor;
    private readonly string fontName;
    private readonly string text;
    public ButtonPrefab(Color hoverColor, Color pressedColor, Color defaultColor, Color txtColor, string fontName, string text)
    {
        this.hoverColor = hoverColor;
        this.pressedColor = pressedColor;
        this.defaultColor = defaultColor;
        this.txtColor = txtColor;
        this.fontName = fontName;
        this.text = text;
    }
    public Entity Instantiate(BaseGame game)
    {
        var spr = new Sprite
        {
            TextureName = "_pixel",
            Source = new Rectangle(0, 0, 1, 1),
            Tint = defaultColor,
            Offset = Vector2.Zero,
        };

        var ent = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(2, 2),
                Size = new Vector2(100, 50),
                Layer = 0.9f,
            })
            .With(spr)
            .With(new Text
            {
                FontName = fontName,
                Content = text,
                Offset = Vector2.Zero,
                Tint = txtColor
            })
            .With(new MouseInput
            {
                OnMouseEnter = () => { spr.Tint = hoverColor; },
                OnMouseLeave = () => { spr.Tint = defaultColor; },
                OnMousePressed = (mb) => //todo
                {
                    //todo
                    if (mb == Systems.MouseButton.Left)
                        spr.Tint = pressedColor;
                },
            });

        return ent;
        
        
    }
}

//    public Rectangle Rect { get; set; }
//    public Color Color { get; set; }
//    public Color TextColor { get; set; }
//    public Color HighlightColor { get; set; }
//    public Color HighlightTextColor { get; set; }
//    public SpriteFont Font { get; set; }
//    public bool Filled { get; set; }
//    public string Text { get; set; }
//    public float TextScale { get; set; }
//    public Point TextOffset { get; set; }
//    public bool Hovering { get; set; }

//    public delegate void OnClickEventHandler(object sender, ClickEventArgs e);
//    public event OnClickEventHandler OnClick;

//    public Entity Instantiate(BaseGame game)
//    {
//        return default;
//    }
//    public Button(Entity entity) : base(entity)
//    {
//    }


//    public override void Update(float dt)
//    {
//        Hovering = Rect.Contains(Owner.Game.MouseState.Position);

//        if (Hovering && Owner.Game.MouseState.WasButtonJustUp(MouseButton.Left))
//            OnClick?.Invoke(this, new ClickEventArgs(MouseButton.Left));
//    }

//    public override void Draw(SpriteBatch sb)
//    {
//        if (Filled)
//            sb.FillRectangle(Rect, Hovering ? HighlightColor : Color);
//        else
//            sb.DrawRectangle(Rect, Hovering ? HighlightColor : Color);

//        sb.DrawString(Font, Text, (Rect.Location + TextOffset).ToVector2(),Hovering ? HighlightTextColor : TextColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
//    }

//}

//public class ClickEventArgs
//{
//    public ClickEventArgs(MouseButton mouseButton) { MouseButton = mouseButton; }
//    public MouseButton MouseButton { get; }
//}