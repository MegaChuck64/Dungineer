using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Dungineer.Prefabs.Entities;
using Engine;
using System;

namespace Dungineer.Behaviors;

public class DropOnDeath : IBehavior
{
    private readonly MapObjectType[] mapObjectLottery;
    public DropOnDeath(params MapObjectType[] mapObjLottery)
    {
        mapObjectLottery = mapObjLottery;
    }

    public bool TryPerform(Entity performer, Entity inflicted)
    {
        if (mapObjectLottery.Length == 0) 
            return false;

        if (performer.GetComponent<MapObject>() is MapObject mapObj)
        {
            var newType = mapObjectLottery[BaseGame.Rand.Next(0, mapObjectLottery.Length)];
            Entity newEnt = newType switch
            {
                MapObjectType.Arcanium => MapItemsPrefab.CreateAracanium(mapObj.MapX, mapObj.MapY),
                MapObjectType.HealthPotion => MapItemsPrefab.CreateHealthPotion(mapObj.MapX, mapObj.MapY),
                _ => throw new NotImplementedException($"DropOnDeath behavior not implemented for {newType}."),
            };
            SceneManager.AddEntity(SceneManager.CurrentScene, newEnt);
            return true;

        }

        return false;
    }
}