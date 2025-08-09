using Dungineer.Components.GameWorld;
using Engine;
using System;

namespace Dungineer.Behaviors;

public class Collect : IBehavior
{
    public bool TryPerform(Entity performer, Entity inflicted)
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
                            return true;
                        case Models.MapObjectType.HealthPotion:
                            if (stats.Health >= stats.MaxHealth)
                            {
                                return false; // No need to collect if already at max health
                            }
                            var max = stats.Health;
                            var val = MainGame.Rand.Next(1, max + 1);
                            stats.Health =
                                stats.Health + val > stats.MaxHealth ?
                                stats.MaxHealth :
                                stats.Health + val;
                            return true;
                        case Models.MapObjectType.ManaPotion:
                            if (stats.Mana >= stats.MaxMana)
                            {
                                return false; // No need to collect if already at max mana
                            }
                            var maxMana = stats.Mana;
                            var manaVal = MainGame.Rand.Next(1, maxMana + 1);
                            stats.Mana =
                                stats.Mana + manaVal > stats.MaxMana ?
                                stats.MaxMana :
                                stats.Mana + manaVal;
                            return true;
                        default:
                            throw new NotImplementedException($"Collecting behavior not implemented for {collectable.Type}.");
                    }
                }
            }
        }

        return false;
    }
}