using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using GameCode.Screens;
using GameCode.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode;

public class MainGame : BaseGame
{
    public FPSCounter FPSCounter { get; set; }
    public ScreenManager Screens { get; set; }
    public MainGame() : base()
    {
        Screens = new ScreenManager();
        Components.Add(Screens);
    }
    public override void Init()
    {
        //ToggleFullscreen();
    }
    public override void Load(ContentManager content)
    {
        Screens.LoadScreen(new MenuScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
        FPSCounter = new FPSCounter(this)
        {
            Font = ContentLoader.LoadFont("consolas_22", Content),
        };

        TileLoader.Load(Content, Rand);
    }

    public override void OnUpdate(GameTime gameTime)
    {
        FPSCounter.Tick(gameTime);
        if (KeyState.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.OemTilde))
        {
            Debug = !Debug;
        }
    }

    public override void OnDraw(GameTime gameTime)
    {
        if (!Debug) return;

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);
        FPSCounter.Draw(SpriteBatch);
        SpriteBatch.End();
    }
}
