namespace Hopper.Utils.Chains
{
    public interface IChain : ICopyable 
    {
        bool IsEmpty { get; }
    }

    public interface IChain<T> : IChain 
    {
        void Add(T handler);
    }
}