using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace GameCode.Screens;

public class DemoScreen : BaseScreen
{
    MainGame game => Game as MainGame;
    
    FPSCounter fpsCounter;
    Button backButton;
    WeatherSystem weatherSystem;
    Player player;    

    public DemoScreen(MainGame game) : base(game, "consolas_22")
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();

        var groundTexture = Game.Content.Load<Texture2D>(@"Sprites\ground");
        var treeTexture = Game.Content.Load<Texture2D>(@"Sprites\tree");
        var playerTexture = Game.Content.Load<Texture2D>(@"Sprites\player");


        EntityManager.AddEntity(
            new Sprite(
                game,
                treeTexture,
                new Vector2(100, game.Height - groundTexture.Height - treeTexture.Height + 10)));

        EntityManager.AddEntity(
            new Sprite(
                game,
                groundTexture,
                new Vector2(-groundTexture.Width, game.Height - groundTexture.Height)));
        
        EntityManager.AddEntity(
            new Sprite(
                game,
                groundTexture,
                  new Vector2(0, game.Height - groundTexture.Height)));

        EntityManager.AddEntity(
            new Sprite(
                game,
                groundTexture,
                  new Vector2(groundTexture.Width, game.Height - groundTexture.Height)));

        weatherSystem = 
            new WeatherSystem(
                game, 
                WeatherSystem.WeatherState.Snowing, 
                game.Height - groundTexture.Height, 
                EntityManager);

        player = 
            new Player(
                game,
                new Vector2(
                    game.Width/2 - playerTexture.Width/2, 
                    game.Height - groundTexture.Height - playerTexture.Height + 16f));

        EntityManager.AddEntity(player);

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
        //dont add to entity system, we want to draw it last
        //entityManager.AddEntity(fpsCounter);
    }

    private void BackButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new MenuScreen(game), new FadeTransition(GraphicsDevice, Color.Black, 2f));

    }

    public override void Update(GameTime gameTime)
    {
        player.Update(gameTime.GetElapsedSeconds());
        
        weatherSystem.Update(gameTime.GetElapsedSeconds());
        backButton.Update(gameTime.GetElapsedSeconds());
        fpsCounter.Tick(gameTime);

        if (game.KeyState.WasKeyJustDown(Keys.R))
        {
            weatherSystem.State = WeatherSystem.WeatherState.Raining;
        }
        if (game.KeyState.WasKeyJustDown(Keys.S))
        {
            weatherSystem.State = WeatherSystem.WeatherState.Snowing;
        }


        Camera.Position = new Vector2(player.Transform.Position.X - game.Width / 2, Camera.Position.Y);

        if (Camera.Position.X > game.Width)
            Camera.Position = new Vector2(game.Width, Camera.Position.Y);

        if (Camera.Position.X < -game.Width)
            Camera.Position = new Vector2(-game.Width, Camera.Position.Y);
        
    }

    public override void Draw(GameTime gameTime)
    {



        //player.Draw(game.SpriteBatch);

        base.Draw(gameTime);


        //UI        
        game.SpriteBatch.Begin();

        player.DrawUI(game.SpriteBatch);
        fpsCounter.Draw(game.SpriteBatch);
        backButton.Draw(game.SpriteBatch);

        //mouse pointer on top of everything
        game.SpriteBatch.DrawCircle(new CircleF(game.MouseState.Position, 4f), 10, Color.Green);



        game.SpriteBatch.End();
    }

}