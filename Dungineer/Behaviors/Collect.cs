using Dungineer.Components.GameWorld;
using Engine;

namespace Dungineer.Behaviors;

public class Collect : IBehavior
{
    public void Perform(Entity performer, Entity inflicted)
    {
        if (performer.GetComponent<CreatureStats>() is CreatureStats stats)
        {
            if (inflicted.GetComponent<MapObject>() is MapObject collectable)
            {
                var info = Settings.MapObjectAtlas[collectable.Type];
                if (info.Collectable)
                {
                    switch (collectable.Type)
                    {
                        case Models.MapObjectType.Arcanium:
                            stats.Money += MainGame.Rand.Next(1, 10);
                            break;
                    }
                }
            }
        }
    }
}