using Engine;

namespace Dungineer.Components;

//used like a flag basically 
public class Cursor : Component
{
    public Cursor(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}