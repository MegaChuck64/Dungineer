using System.Collections.Generic;
using System.Linq;

namespace Engine;

public class Entity
{
    public List<Component> Components { get; private set; }

    public List<string> Tags { get; private set; } = new List<string>();
    public Entity(params Component[] comps)
    {
        Components =
            comps == null ?
            new List<Component>() : comps.ToList();
    }

    public T GetComponent<T>() where T : Component =>
        Components.OfType<T>().FirstOrDefault();

    public IEnumerable<T> GetComponents<T>() where T : Component =>
        Components.OfType<T>();

    public Entity With<T>(T comp) where T : Component
    {
        Components.Add(comp);
        return this;
    }

    public Entity WithTag(string tag)
    {
        Tags.Add(tag);
        return this;
    }

    public bool HasTag(string val) =>
        Tags.Any(t => t == val);

}

