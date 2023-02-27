using Dungineer.Components.GameWorld;
using Engine;

namespace Dungineer.Behaviors;

public class BasicAttack : IBehavior
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
}