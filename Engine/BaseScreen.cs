using GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;

namespace Engine;

public class BaseScreen : GameScreen
{
    public BaseGame BGame => Game as BaseGame;
    public SpriteFont Font { get; set; }
    public OrthographicCamera Camera { get; set; }
    public EntityManager EntityManager { get; set; }
    public string FontName { get; private set; }
    public BaseScreen(Game game, string fontName) : base(game)
    {
        EntityManager = new EntityManager();
        FontName = fontName;
    }

    public override void LoadContent()
    {
        base.LoadContent();
        Font = Game.Content.Load<SpriteFont>(@$"Fonts\{FontName}");

        var viewportAdapter = new BoxingViewportAdapter(Game.Window, GraphicsDevice, BGame.Width, BGame.Height);
        Camera = new OrthographicCamera(viewportAdapter);
    }

    public override void Update(GameTime gameTime)
    {
        EntityManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        var transformMatrix = Camera.GetViewMatrix();
        BGame.SpriteBatch.Begin(
            transformMatrix: transformMatrix, 
            samplerState: SamplerState.PointWrap);

        EntityManager.Draw((Game as MainGame).SpriteBatch);

        BGame.SpriteBatch.End();

    }
}
