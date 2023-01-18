using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameCode.Screens;

public class TestScreen : BaseScreen
{
    MapPrinter printer;
    public TestScreen(Game game) : base(game, "consolas_22")
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();
        printer = new MapPrinter(Content)
        {
            Map = new Map(10, 10, BGame.Rand),
            Camera = Camera,
            TileSize = 32,
        };
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var transformMatrix = Camera.GetViewMatrix();
        BGame.SpriteBatch.Begin(
            transformMatrix: transformMatrix,
            samplerState: SamplerState.PointWrap,
            sortMode: SpriteSortMode.FrontToBack);
        
        printer.Draw(BGame.SpriteBatch);
        
        BGame.SpriteBatch.End();
    }
}