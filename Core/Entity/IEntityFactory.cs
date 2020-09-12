using Core.Behaviors;

namespace Core
{
    public interface IEntityFactory
    {
        public Entity Instantiate();
        public void AddBehavior<Beh>(BehaviorConfig conf)
            where Beh : Behavior, new();
        public void RetouchAndSave(Retoucher retoucher);
        public bool IsRetouched(Retoucher retoucher);
    }
}