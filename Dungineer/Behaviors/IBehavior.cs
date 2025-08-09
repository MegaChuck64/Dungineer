using Engine;

namespace Dungineer.Behaviors;

public interface IBehavior
{
    public bool TryPerform(Entity performer, Entity inflicted);

}