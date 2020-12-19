namespace Hopper.Utils.Chains
{
    public interface ILinearChain<Event> where Event : EventBase
    {
        void AddHandler(System.Action<Event> handlerFunction);
    }
}