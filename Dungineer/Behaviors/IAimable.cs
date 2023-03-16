using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public interface IAimable
{
    public IEnumerable<Point> Aim(Entity ent);
}