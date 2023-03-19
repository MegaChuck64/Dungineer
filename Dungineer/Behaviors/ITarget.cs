using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Behaviors;

public interface ITarget : IBehavior
{
    public Point Target { get; }
    public void SetTarget(Point target);

}