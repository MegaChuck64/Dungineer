using Dungineer.Components.GameWorld;
using Engine;
using Engine.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Prefabs.Entities;

public class MapItemsPrefab : IPrefab<List<Entity>>
{
    private Map map;
    public MapItemsPrefab(Map map)
    {
        this.map = map;
    }
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>
        {
            //CreatePlayer(game)
        };

        for (int i = 0; i < 3; i++)
        {
            ents.Add(
                CreateGhost(
                    game,
                    ents
                        .Where(t => t.Components.Any(g => g is MapObject))
                        .Select(b => b.GetComponent<MapObject>())
                        .ToArray()));
        }
        return ents;
    }


    private Entity CreateGhost(BaseGame game, params MapObject[] mapObjs)
    {
        var ghostPos = map.GetRandomEmptyTile(game, mapObjs);
        var ent = new Entity(game)
            .With(new MapObject
            {
                MapX = ghostPos.x,
                MapY = ghostPos.y,
                Tint = Color.White,
                Type = MapObjectType.Ghost
            })
            .With(new CreatureStats
            {
                Health = 10,
                MaxHealth = 10,

                Stamina = 10,
                MaxStamina = 10,

                MoveSpeed = 0.75f,
            })
            .With(new Tag
            {
                Value = "Ghost"
            });

        return ent;
    }
    private Entity CreatePlayer(BaseGame game)
    {

        var ent = new Entity(game)
            .With(new MapObject
            {
                MapX = map.GroundTiles.GetLength(0) / 2,
                MapY = map.GroundTiles.GetLength(1) / 2,
                Tint = Color.White,
                Type = MapObjectType.Human,
            })
            .With(new CreatureStats
            {
                Health = 20,
                MaxHealth = 20,

                Stamina = 20,
                MaxStamina = 20,

                MoveSpeed = 1f
            })
            .With(new Tag
            {
                Value = "Player"
            });
        return ent;
    }
}
