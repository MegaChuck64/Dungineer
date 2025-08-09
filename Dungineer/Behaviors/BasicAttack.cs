using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public class BasicAttack : ISpell
{
    public Point Target { get; private set; }

    public SpellType GetSpellType() => SpellType.BasicAttack;

    public void SetTarget(Point target)
    {
        Target = target;
    }

    public bool TryPerform(Entity performer, Entity inflicted)
    {
        var performerStats = performer.GetComponent<CreatureStats>();
        var spellInfo = Settings.SpellAtlas[GetSpellType()];

        if (performerStats == null || performerStats.Mana < spellInfo.ManaCost)
            return false;

        if (inflicted.GetComponent<CreatureStats>() is CreatureStats targetStats)
        {
            performerStats.Mana -= spellInfo.ManaCost;
            if (performerStats.Mana < 0)
            {
                performerStats.Mana = 0;
            }
            if (performerStats.Mana > performerStats.MaxMana)
            {
                performerStats.Mana = performerStats.MaxMana;
            }

            targetStats.Health -= performerStats.Strength;
            if (targetStats.Health < 0)
            {
                targetStats.Health = 0;
            }

            return true;
        }

        return false;
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
}