using Dungineer.Components.GameWorld;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public class BasicAttack : ISpell, IBehavior
{
    public Entity Target { get; set; }
    public BasicAttack(Entity target)
    {
        Target = target;
    }
    public void Perform(Entity ent)
    {
        var performerStats = ent.GetComponent<CreatureStats>();


        if (Target.GetComponent<CreatureStats>() is CreatureStats targetStats)
        {
            targetStats.Health -= performerStats.Strength;
            if (targetStats.Health < 0)
            {
                targetStats.Health = 0;
            }
        }

    }

    public IEnumerable<Point> Aim(Entity ent)
    {
        var points = new List<Point>();

        var mapObj = ent.GetComponent<MapObject>();
        if (ent.GetComponent<CreatureStats>() is CreatureStats stats)
        {
            for (int x = mapObj.MapX - (int)stats.AttackRange; x < mapObj.MapX + (int)stats.AttackRange; x++)
            {
                for (int y= mapObj.MapY - (int)stats.AttackRange; y < mapObj.MapY + (int)stats.AttackRange; y++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(mapObj.MapX, mapObj.MapY)) <= stats.AttackRange)
                    {
                        points.Add(new Point(x,y));
                    }    
                }
            }
        }

        return points;
    }
}