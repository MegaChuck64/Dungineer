using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine;

public abstract class BaseSystem
{
    public BaseGame Game { get; private set; }
    public BaseSystem(BaseGame game)
    {
        Game = game;
    }

    public abstract void Update(GameTime gameTime, IEnumerable<Entity> entities);
    public abstract void Draw(GameTime gameTime, IEnumerable<Entity> entities);

    //public static IEnumerable<T> GetComponents<T>(IEnumerable<Entity> entities) where T : Component =>
    //    entities
    //        .Where(t => t.Components.Any(c => c.GetType() == typeof(T)))
    //        .Select(b => b.Components.OfType<T>())
    //        .Cast<T>();
    
}