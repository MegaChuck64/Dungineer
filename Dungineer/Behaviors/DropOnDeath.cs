using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Dungineer.Prefabs.Entities;
using Engine;
using System;

namespace Dungineer.Behaviors;

public class DropOnDeath : IBehavior
{
    private readonly MapObjectType[] mapObjectLottery;
    private Random random;
    public DropOnDeath(Random rand, params MapObjectType[] mapObjLottery)
    {
        mapObjectLottery = mapObjLottery;
        random = rand;
    }

    public void Perform(Entity performer, Entity inflicted)
    {
        if (mapObjectLottery.Length == 0) return;

        if (performer.GetComponent<MapObject>() is MapObject mapObj)
        {
            var newType = mapObjectLottery[random.Next(0, mapObjectLottery.Length)];

            Entity newEnt;
            switch (newType)
            {
                case MapObjectType.Arcanium:
                    newEnt = MapItemsPrefab.CreateAracanium(mapObj.MapX, mapObj.MapY);
                    break;
                case MapObjectType.HealthPotion:
                    newEnt = MapItemsPrefab.CreateHealthPotion(mapObj.MapX, mapObj.MapY);
                    break;
                default:
                    return;
            }

            SceneManager.AddEntity(SceneManager.CurrentScene, newEnt);
        }


    }
}