namespace Chains
{
    public class EvHandler<Event> : IEvHandler<Event> where Event : EventBase
    {
        public int Priority { get; set; } = (int)PriorityRanks.Medium;
        private System.Action<Event> handlerFunction;

        public EvHandler(System.Action<Event> handlerFunc, PriorityRanks priority = PriorityRanks.Default)
        {
            handlerFunction = handlerFunc;
            this.Priority = (int)priority;
        }

        public void Call(Event ev)
        {
            handlerFunction(ev);
        }
    }
}