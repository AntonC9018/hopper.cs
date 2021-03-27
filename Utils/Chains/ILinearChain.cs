namespace Hopper.Utils.Chains
{
    public interface ILinearChain<Context> where Context : ContextBase
    {
        void AddHandler(System.Action<Context> handlerFunction);
    }
}