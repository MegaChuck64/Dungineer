using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;

namespace GameCode.Screens;
public class CrawlScreen : BaseScreen
{
    MainGame game => Game as MainGame;

    FPSCounter fpsCounter;
    Button backButton;
    Map map;
    public CrawlScreen(MainGame game) : base(game, "consolas_22")
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();
       
        fpsCounter = new FPSCounter(game)
        {
            Font = Font,
        };

        backButton = new Button(game)
        {
            Color = Color.Red,
            HighlightColor = Color.Blue,
            TextColor = Color.Green,
            HighlightTextColor = Color.Blue,
            Filled = false,
            Font = Font,
            Text = "Back",
            TextScale = 1f,
            Rect = new Rectangle(10, 10, 100, 40),
            TextOffset = new Point(17, 6),
        };

        backButton.OnClick += BackButton_OnClick;

       
        var viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, game.Width, game.Height);
        
        map = new Map(game);
        EntityManager.AddEntity(map);
        //eventHistory = new EventHistory(game);
        //entityManager.AddEntity(eventHistory);
    }

    private void BackButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new MenuScreen(game), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        backButton.Update(gameTime.GetElapsedSeconds());
        fpsCounter.Tick(gameTime);

    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        //UI        
        game.SpriteBatch.Begin();

        fpsCounter.Draw(game.SpriteBatch);
        backButton.Draw(game.SpriteBatch);

        game.SpriteBatch.DrawCircle(new CircleF(game.MouseState.Position, 4f), 10, Color.Blue);

        game.SpriteBatch.End();
    }
}
