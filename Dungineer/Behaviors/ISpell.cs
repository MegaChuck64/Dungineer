using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Behaviors;

public interface ISpell : ITarget
{
    public IEnumerable<Point> Aim(Entity ent);

    public SpellType GetSpellType();
}