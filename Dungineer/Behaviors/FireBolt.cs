using Dungineer.Behaviors.Effects;
using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public class FireBolt : ISpell
{
    public Point Target { get; private set; }
    public SpellType GetSpellType() => SpellType.FireBolt;

    public void SetTarget(Point target)
    {
        Target = target;
    }

    public IEnumerable<Point> Aim(Entity ent)
    {
        var points = new List<Point>();

        var mapObj = ent.GetComponent<MapObject>();

        var spellInfo = Settings.SpellAtlas[GetSpellType()];

        for (int x = mapObj.MapX - spellInfo.Range; x < mapObj.MapX + spellInfo.Range + 1; x++)
        {
            for (int y = mapObj.MapY - spellInfo.Range; y < mapObj.MapY + spellInfo.Range + 1; y++)
            {
                if (x == mapObj.MapX && y == mapObj.MapY)
                    continue;

                if (Vector2.Distance(new Vector2(x, y), new Vector2(mapObj.MapX, mapObj.MapY)) <= spellInfo.Range)
                {
                    points.Add(new Point(x, y));
                }
            }
        }

        return points;
    }


    public void Perform(Entity performer, Entity inflicted)
    {
        var performerStats = performer.GetComponent<CreatureStats>();

        if (inflicted.GetComponent<CreatureStats>() is CreatureStats targetStats)
        {
            targetStats.Health -= performerStats.Strength;
            if (targetStats.Health <= 0)
            {
                targetStats.Health = 0;
            }
            else
            {
                if (inflicted.GetComponent<EffectController>() is EffectController targetedEffectController)
                {
                    targetedEffectController.Effects.Add(new FireEffect());
                }
            }

        }
    }

}
    

