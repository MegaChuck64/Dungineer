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
            for (int y= 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var mapObjs = SceneManager.ComponentsOfType<MapObject>();

                if (map.IsEmpty(x, y, mapObjs.ToArray()))
                {
                    if (game.Rand.NextSingle() <= itemSpawnRate)
                    {
                        var itemType = mapItemLottery[game.Rand.Next(mapItemLottery.Count)];
                        switch (itemType)
                        {
                            case MapObjectType.Human:
                                break;
                            case MapObjectType.Ghost:
                                ents.Add(CreateGhost(x, y));
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


    private static Entity CreateGhost(int x, int y)
    {
        var ent = new Entity()
            .With(new MapObject
            {
                MapX = x,
                MapY = y,
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

                Strength = 1,

                SightRange = 4,
                AttackRange = 1,

                Money = 0,

            })
            .WithTag("Ghost");

        return ent;
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
