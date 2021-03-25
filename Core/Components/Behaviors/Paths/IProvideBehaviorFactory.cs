namespace Hopper.Core.Components
{
    public interface IProvideBehaviorFactory
    {
        IBehaviorFactory<T> GetBehaviorFactory<T>() where T : Behavior, new();
        bool HasBehaviorFactory<T>() where T : Behavior, new();
    }
}