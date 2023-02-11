using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using GameCode.Entities;
using Engine;
using Engine.Prefabs;
using Engine.Components;
using MonoGame.Extended.Input;

namespace GameCode.Screens;

public class MenuScreen : BaseScreen
{
    Entity playButton;

    public MenuScreen(MainGame game) : base(game, "consolas_22")
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();

        //var testButtonEntity = new Entity(BGame);
        //testButton = new Button(testButtonEntity)
        //{
        //    Color = Color.Orange,
        //    HighlightColor = Color.Blue,
        //    TextColor = Color.Red,
        //    HighlightTextColor = Color.Blue,
        //    Filled = false,
        //    Font = Font,
        //    Text = "Play",
        //    TextScale = 1f,
        //    Rect = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - 50, 150, 100, 40),
        //    TextOffset = new Point(16, 6),
        //};
        //testButtonEntity.Components.Add(testButton);

        //testButton.OnClick += TestButton_OnClick;

        var buttonPrefab = new ButtonPrefab(Color.Orange, Color.Blue, Color.Green);
        playButton = buttonPrefab.Instantiate(BGame);
        var mouseInput = playButton.GetComponent<MouseInput>();
        mouseInput.OnMouseReleased = PlayButton_OnClick;
        EntityManager.Entities.Add(playButton);
    }

    private void PlayButton_OnClick(MouseButton mb)
    {
        ScreenManager.LoadScreen(new CharacterSelectScreen(Game), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }


    public override void Update(GameTime gameTime)
    {        
        //testButton.Update(gameTime.GetElapsedSeconds());
    }


    public override void Draw(GameTime gameTime)
    {
        //BGame.SpriteBatch.Begin();

        //testButton.Draw(BGame.SpriteBatch);
        //BGame.SpriteBatch.DrawCircle(new CircleF(BGame.MouseState.Position, 4f), 10, Color.Green);

        //BGame.SpriteBatch.End();

    }

}