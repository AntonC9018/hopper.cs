namespace Hopper.Core.Components
{
    public interface IBehaviorFactory<out T> : IWithChainTemplate
         where T : Behavior
    {
        T Instantiate(Entity entity);
    }
}