using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using GameCode.Entities;
using Engine;

namespace GameCode.Screens;

public class MenuScreen : BaseScreen
{
    Button testButton;

    public MenuScreen(MainGame game) : base(game, "consolas_22")
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();

        testButton = new Button(Game as MainGame)
        {
            Color = Color.Orange,
            HighlightColor = Color.Blue,
            TextColor = Color.Red,
            HighlightTextColor = Color.Blue,
            Filled = false,
            Font = Font,
            Text = "Play",
            TextScale = 1f,
            Rect = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - 50, 150, 100, 40),
            TextOffset = new Point(16, 6),
        };

        testButton.OnClick += TestButton_OnClick;

    }

    private void TestButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new CharacterSelectScreen(Game), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }


    public override void Update(GameTime gameTime)
    {        
        testButton.Update(gameTime.GetElapsedSeconds());
    }


    public override void Draw(GameTime gameTime)
    {
        BGame.SpriteBatch.Begin();

        testButton.Draw(BGame.SpriteBatch);
        BGame.SpriteBatch.DrawCircle(new CircleF(BGame.MouseState.Position, 4f), 10, Color.Green);

        BGame.SpriteBatch.End();

    }

}