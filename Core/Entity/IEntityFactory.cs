namespace Core
{
    public interface IFactory<out T> : IHaveId
    {
        T Instantiate();
        T ReInstantiate(int id);
    }
    // public interface IEntityFactory
    // {

    //     public IEntityFactory AddBehavior<Beh>(BehaviorConfig conf)
    //         where Beh : Behavior, new();
    //     public IEntityFactory RetouchAndSave(Retoucher retoucher);
    //     public bool IsRetouched(Retoucher retoucher);
    // }
}