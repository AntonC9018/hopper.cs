namespace Hopper.Core
{
    public interface IFactory<out T> : IKind
    {
        T Instantiate(Registry registry);
        T ReInstantiate(Registry registry, int id);
    }
}