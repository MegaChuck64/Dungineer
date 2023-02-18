namespace Engine;

public abstract class Component
{
    public bool IsActive { get; set; }

    public Component(bool isActive = true)
    {
        IsActive = isActive;
    }

}