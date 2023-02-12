using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class UISystem : BaseSystem
{
    //private readonly SpriteBatch sb;
    public UISystem(BaseGame game) : base(game)
    {
        //sb = new SpriteBatch(game.GraphicsDevice);
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var player = entities.FirstOrDefault(t => t.Components.Any(c => c is Player))?.GetComponent<Player>();
        if (player == null) return;

        foreach (var ent in entities)
        {            
            if (ent.HasTag("Stat Panel"))
            {
                if (ent.GetComponent<Text>() is Text text)
                {
                    text.Content = player.Name + " \\n" + player.Health + "/" + player.MaxHealth;
                }
            }

            if (ent.HasTag("Portrait"))
            {
                if (ent.GetComponent<Sprite>() is Sprite spr)
                {
                    spr.Source = new Rectangle(player.PotraitIndex * 512, 0, 512, 512);
                }
            }
        }
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //sb.Begin(
        //    sortMode: SpriteSortMode.FrontToBack,
        //    blendState: BlendState.NonPremultiplied,
        //    samplerState: SamplerState.PointClamp,
        //    depthStencilState: DepthStencilState.DepthRead,
        //    rasterizerState: RasterizerState.CullCounterClockwise,
        //    effect: null,
        //    transformMatrix: null); //camera here todo... probably no ui camera needed actually

        //var player = entities.FirstOrDefault(t => t.Components.Any(c => c is Player))?.GetComponent<Player>();
        //if (player == null) return;

        //foreach (var ent in entities)
        //{
        //    if (ent.GetComponent<StatPanel>() is StatPanel statPanel)
        //    {
        //        sb.Draw()
        //    }
        //}

        //sb.End();
    }
}