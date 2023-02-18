using Engine.Components;
using System.Collections.Generic;
using System.Linq;

namespace Engine;

public class Entity
{
    public BaseGame Game { get; private set; }

    public List<Component> Components { get; private set; }
    public Entity(BaseGame game, params Component[] comps)
    {
        Game = game;
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
    
    public bool HasTag(string val) => 
        Components.Any(t => t is Tag tag && tag.Value == val);
    
}

