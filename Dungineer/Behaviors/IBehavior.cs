using Engine;

namespace Dungineer.Behaviors;

public interface IBehavior
{
    public void Perform(Entity performer, Entity inflicted);

}