namespace Engine;

public interface IPrefab<T>
{
    public T Instantiate(BaseGame game);
}