using Dungineer.Behaviors;
using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Prefabs.Entities;

public class MapItemsPrefab : IPrefab<List<Entity>>
{
    private readonly Map map;
    public MapItemsPrefab(Map map)
    {
        this.map = map;
    }
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();

        var itemSpawnRate = 0.06f;
        var mapItemLottery = new List<MapObjectType>();
        foreach (var mapObj in Settings.MapObjectAtlas)
        {
            for (int i = 0; i < mapObj.Value.LotteryValue; i++)
            {
                mapItemLottery.Add(mapObj.Key);
            }
        }

        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var mapObjs = SceneManager.ComponentsOfType<MapObject>();

                if (map.IsEmpty(x, y, mapObjs.ToArray()))
                {
                    if (MainGame.Rand.NextSingle() <= itemSpawnRate)
                    {
                        var itemType = mapItemLottery[MainGame.Rand.Next(mapItemLottery.Count)];
                        switch (itemType)
                        {
                            case MapObjectType.Human:
                                break;
                            case MapObjectType.Ghost:
                                ents.Add(CreateCreature(x, y, MapObjectType.Ghost, "Ghost"));
                                break;
                            case MapObjectType.Arcanium:
                                ents.Add(CreateAracanium(x, y));
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
        }

        return ents;
    }

    private static Entity CreateCreature(int x, int y, MapObjectType mapObjectType, string tag)
    {
        var info = Settings.MapObjectAtlas[mapObjectType];

        var behaviorController = new BehaviorController();
        var spellBook = new SpellBook();

        foreach (var beh in info.Behaviors)
        {
            var spellInfos = Settings.SpellAtlas.Where(t => t.Value.Name == beh).ToArray();
            if (spellInfos.Length > 0)
            {
                var spellInfo = spellInfos[0];
                switch (spellInfo.Value.Name)
                {
                    case "Basic Attack":
                        spellBook.Spells.Add(new BasicAttack());
                        break;
                }
            }
            else
            {
                switch (beh)
                {
                    case "Basic Movement":
                        behaviorController.Behaviors.Add(new BasicMovement());
                        break;
                }
            }


        }


        return new Entity()
            .With(new MapObject
            {
                MapX = x,
                MapY = y,
                Tint = Color.White,
                Type = mapObjectType
            })
            .With(info.Stats.Clone() as CreatureStats)
            .With(behaviorController)
            .With(spellBook)
            .WithTag(tag);
    }
    public static Entity CreateAracanium(int x, int y)
    {
        var ent = new Entity()
            .With(new MapObject
            {
                MapX = x,
                MapY = y,
                Tint = Color.White,
                Type = MapObjectType.Arcanium
            })
            .WithTag("Arcanium");

        return ent;
    }

}
