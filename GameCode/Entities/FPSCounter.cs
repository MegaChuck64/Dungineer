using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameCode.Entities;

public class FPSCounter : Component
{
    int frameRate = 0;
    int frameCounter = 0;
    TimeSpan elapsedTime = TimeSpan.Zero;
    public SpriteFont Font { get; set; }
    
    public FPSCounter(Entity owner, bool tickOnUpdate = false) : base(owner)
    {
    }

    public override void Update(float dt)
    {

    }

    public void Tick(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime;

        if (elapsedTime > TimeSpan.FromSeconds(1))
        {
            elapsedTime -= TimeSpan.FromSeconds(1);
            frameRate = frameCounter;
            frameCounter = 0;
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        frameCounter++;

        var fps = $"FPS: {frameRate}";
        sb.DrawString(Font, fps, new Vector2(4, sb.GraphicsDevice.Viewport.Height - 40), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
        
    }
}