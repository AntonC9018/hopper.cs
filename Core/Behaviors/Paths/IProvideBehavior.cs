namespace Hopper.Core.Behaviors
{
    public interface IProvideBehavior
    {
        T Get<T>() where T : Behavior, new();
        T TryGet<T>() where T : Behavior, new();
    }
}