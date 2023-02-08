using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine;

public class Entity
{
    public bool IsDestroyed { get; private set; }
    public BaseGame Game { get; private set; }

    public List<Component> Components { get; private set; }
    public Entity(BaseGame game)
    {
        IsDestroyed = false;
        Game = game;
        Components = new List<Component>();
    }

    public void Update(float dt)
    {
        foreach (var comp in Components)
        {
            comp.Update(dt);
        }
    }

    public void Draw(SpriteBatch sb)
    {
        foreach (var comp in Components)
        {
            comp.Draw(sb);
        }
    }
    public void Destroy()
    {
        IsDestroyed = true;
    }
}

public abstract class Component
{
    public Entity Owner { get; set; }
    public bool IsActive { get; set; }

    public Component(Entity owner, bool isActive = true)
    {
        Owner = owner;
        IsActive = isActive;
    }

    public abstract void Update(float dt);

    public abstract void Draw(SpriteBatch sb);
}