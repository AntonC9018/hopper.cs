using Core.Behaviors;

namespace Core
{
    public interface IEntityFactory : IHaveId
    {
        Entity Instantiate();
        Entity ReInstantiate(int id);
    }
    // public interface IEntityFactory
    // {

    //     public IEntityFactory AddBehavior<Beh>(BehaviorConfig conf)
    //         where Beh : Behavior, new();
    //     public IEntityFactory RetouchAndSave(Retoucher retoucher);
    //     public bool IsRetouched(Retoucher retoucher);
    // }
}