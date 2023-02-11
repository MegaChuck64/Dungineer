namespace Engine;

public abstract class Component
{
    public Entity Owner { get; set; }
    public bool IsActive { get; set; }

    public Component(Entity owner, bool isActive = true)
    {
        Owner = owner;
        IsActive = isActive;
        owner.Components.Add(this);
    }

}