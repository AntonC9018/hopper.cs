namespace Core.Behaviors
{
    public interface IBehaviorFactory
    {
        Behavior Instantiate(Entity entity, object conf);
    }
}