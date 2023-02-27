using Dungineer.Components.GameWorld;
using Engine;

namespace Dungineer.Behaviors;

public class Collect : IBehavior
{
    public MapObject MapObject { get; set; }    
    public BaseGame Game { get; set; }
    public Collect (MapObject mapObject, BaseGame game)
    {
        MapObject = mapObject;
        Game = game;
    }
    public void Perform(Entity ent)
    {
        var stats = ent.GetComponent<CreatureStats>();

        if (stats != null)
        {
            var info = Settings.MapObjectAtlas[MapObject.Type];
            if (info.Collectable)
            {
                switch (MapObject.Type)
                {
                    case Models.MapObjectType.Arcanium:
                        stats.Money += Game.Rand.Next(1, 10);
                        break;
                }
            }
        }
    }
}