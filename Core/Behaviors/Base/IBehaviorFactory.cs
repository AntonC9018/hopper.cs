namespace Hopper.Core.Behaviors
{
    public interface IBehaviorFactory<out T> : IProvidesChainTemplate
         where T : Behavior
    {
        T Instantiate(Entity entity);
    }
}