namespace Hopper.Core
{
    public interface IKind : IHaveId
    {
        void RegisterSelf(Registry registry);
    }
}