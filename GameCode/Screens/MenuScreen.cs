﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using GameCode.Entities;
using Engine;

namespace GameCode.Screens;

public class MenuScreen : GameScreen
{
    Button orthButton;
    Button testButton;

    MainGame game => Game as MainGame;
    public MenuScreen(MainGame game) : base(game)
    {

    }

    public override void LoadContent()
    {
        base.LoadContent();
        var font = Game.Content.Load<SpriteFont>(@"Fonts\consolas_22");

        //orthButton = new Button(Game as MainGame)
        //{
        //    Color = Color.Purple,
        //    HighlightColor = Color.Blue,
        //    TextColor = Color.Red,
        //    HighlightTextColor = Color.Blue,
        //    Filled = false,
        //    Font = font,
        //    Text = "Play",
        //    TextScale = 1f,
        //    Rect = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - 50, 100, 100, 40),
        //    TextOffset = new Point(16, 6),
        //};


        testButton = new Button(Game as MainGame)
        {
            Color = Color.Orange,
            HighlightColor = Color.Blue,
            TextColor = Color.Red,
            HighlightTextColor = Color.Blue,
            Filled = false,
            Font = font,
            Text = "Play",
            TextScale = 1f,
            Rect = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - 50, 150, 100, 40),
            TextOffset = new Point(16, 6),
        };

        orthButton.OnClick += OrthButton_OnClick;
        testButton.OnClick += TestButton_OnClick;

    }

    private void TestButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new TestScreen(game), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }

    private void OrthButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new OrthScreen(game), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }


    public override void Update(GameTime gameTime)
    {        
        orthButton.Update(gameTime.GetElapsedSeconds());
        testButton.Update(gameTime.GetElapsedSeconds());
    }


    public override void Draw(GameTime gameTime)
    {
        game.SpriteBatch.Begin();

        orthButton.Draw(game.SpriteBatch);
        testButton.Draw(game.SpriteBatch);
        game.SpriteBatch.DrawCircle(new CircleF(game.MouseState.Position, 4f), 10, Color.Green);

        game.SpriteBatch.End();

    }

}