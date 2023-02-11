using System.Collections.Generic;
using System.Linq;

namespace Engine;

public class Entity
{
    public BaseGame Game { get; private set; }

    public List<Component> Components { get; private set; }
    public Entity(BaseGame game)
    {
        Game = game;
        Components = new List<Component>();
    }

    public T GetComponent<T>() where T : Component => 
        Components.OfType<T>().FirstOrDefault();

    public IEnumerable<T> GetComponents<T>() where T : Component =>
        Components.OfType<T>();
    
}

