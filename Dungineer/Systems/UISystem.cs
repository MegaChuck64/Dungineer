using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class UISystem : BaseSystem
{
    private readonly SpriteBatch sb;
    int frameRate = 0;
    int frameCounter = 0;
    TimeSpan elapsedTime = TimeSpan.Zero;
    public SpriteFont Font { get; set; }
    public UISystem(BaseGame game, string fontName) : base(game)
    {
        sb = new SpriteBatch(game.GraphicsDevice);
        Font = ContentLoader.LoadFont(fontName, game.Content);
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var playerStats = entities.FirstOrDefault(t => t.HasTag("Player"))?.GetComponent<CreatureStats>();
        if (playerStats == null) return;

        foreach (var ent in entities)
        {            
            if (ent.HasTag("Stat Panel"))
            {
                if (ent.GetComponent<Text>() is Text text)
                {
                    text.Content = playerStats.Health + "/" + playerStats.MaxHealth;
                }
            }

            //if (ent.HasTag("Portrait"))
            //{
            //    if (ent.GetComponent<Sprite>() is Sprite spr)
            //    {
            //        spr.Source = new Rectangle(player.PotraitIndex * 512, 0, 512, 512);
            //    }
            //}
        }


        elapsedTime += gameTime.ElapsedGameTime;

        if (elapsedTime > TimeSpan.FromSeconds(1))
        {
            elapsedTime -= TimeSpan.FromSeconds(1);
            frameRate = frameCounter;
            frameCounter = 0;
        }
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo... probably no ui camera needed actually

        frameCounter++;

        var fps = $"FPS: {frameRate}";
        sb.DrawString(
            Font, 
            fps, 
            new Vector2(
                4 * Game.WindowRatio, 
                Game.Height - Font.MeasureString(fps).Y - (4 * Game.WindowRatio)), 
            Color.Yellow, 
            0f, 
            Vector2.Zero, 
            1f, 
            SpriteEffects.None, 
            0.9f);



        sb.End();
    }
}