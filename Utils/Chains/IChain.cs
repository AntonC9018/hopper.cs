namespace Hopper.Utils.Chains
{
    public interface IChain : ICopyable {}
    public interface IChain<T> : IChain 
    {
        void Add(T handler);
    }
}