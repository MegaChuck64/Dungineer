using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Components;

public class Cursor : Component
{
    public Cursor(Entity owner, bool isActive = true) : base(owner, isActive)
    {
    }
}