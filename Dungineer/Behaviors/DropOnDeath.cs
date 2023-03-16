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

    public void Perform(Entity ent)
    {
        if (mapObjectLottery.Length == 0) return;
        
        if (ent.GetComponent<MapObject>() is MapObject mapObj)
        {
            var newType = mapObjectLottery[random.Next(0, mapObjectLottery.Length)];

            Entity newEnt;
            switch (newType)
            {
                case MapObjectType.Arcanium:
                    newEnt = MapItemsPrefab.CreateAracanium(mapObj.MapX, mapObj.MapY);
                    break;
                default:
                    return;
            }

            SceneManager.AddEntity(SceneManager.CurrentScene, newEnt);
        }

        
    }
}