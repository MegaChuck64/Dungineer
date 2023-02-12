using Dungineer.Components;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class UISystem : BaseSystem
{
    Player player;
    public UISystem(BaseGame game) : base(game)
    {

    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        player = entities.FirstOrDefault(t => t.Components.Any(c => c is Player))?.GetComponent<Player>();
        if (player == null) return;

        foreach (var ent in entities)
        {            
            if (ent.GetComponent<StatPanel>() is not null)
            {
                if (ent.GetComponent<Text>() is Text text)
                {
                    text.Content = player.Name + " \\n" + player.Health + "/" + player.MaxHealth;
                }
            }
        }
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {

    }
}