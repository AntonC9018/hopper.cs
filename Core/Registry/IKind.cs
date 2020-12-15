namespace Hopper.Core.Registry
{
    public interface IKind : IHaveId
    {
        void RegisterSelf(ModSubRegistry registry);
    }
}