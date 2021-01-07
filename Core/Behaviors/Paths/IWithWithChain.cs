namespace Hopper.Core.Behaviors
{
    public interface IWithWithChain
    {
        T Get<T>() where T : IWithChain;
        T TryGet<T>() where T : IWithChain;
    }
}