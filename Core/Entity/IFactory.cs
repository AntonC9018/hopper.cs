namespace Hopper.Core
{
    public interface IFactory<out T> : IKind, IAfterPatch
    {
        T Instantiate();
    }

    public interface IAfterPatch
    {
        void AfterPatch(Repository repository);
    }
}