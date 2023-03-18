using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public interface ISpell : IBehavior
{
    public Entity Target { get; }
    public void SetTarget(Entity target);
    public IEnumerable<Point> Aim(Entity ent);
}