namespace Chains
{
    public interface ICanAddHandlers<Event> where Event : EventBase
    {
        void AddHandler(EvHandler<Event> handler);
    }
}