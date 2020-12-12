namespace Hopper.Core.Behaviors
{
    public interface IProvideBehaviorFactory
    {
        BehaviorFactory<T> GetBehaviorFactory<T>() where T : Behavior, new();
        bool HasBehaviorFactory<T>() where T : Behavior, new();
    }
}