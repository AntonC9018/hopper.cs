namespace Core
{
    public interface IFactory<out T> : IHaveId
    {
        T Instantiate();
        T ReInstantiate(int id);
    }
}