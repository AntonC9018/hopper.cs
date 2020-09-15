namespace Core
{
    // ( Status -> tinker (-> data) -> flavor )
    public interface IStatus : IHaveId
    {
        // sets up the tinker + flavor on the tinker data
        void Apply(Entity entity, Flavor f);
        // ticks the tinker and removes status if necessary
        void Tick(Entity entity);
        bool IsApplied(Entity entity);
    }
}