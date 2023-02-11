using System.Collections.Generic;
using System.Linq;

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

    public void Destroy()
    {
        IsDestroyed = true;
    }

    public T GetComponent<T>() where T : Component => 
        Components.OfType<T>().FirstOrDefault();

    public IEnumerable<T> GetComponents<T>() where T : Component =>
        Components.OfType<T>();
    
}

