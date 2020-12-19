namespace Hopper.Utils.Chains
{
    public class EvHandler<Event> : IEvHandler<Event> where Event : EventBase
    {
        public int Priority { get; set; } = (int)PriorityRank.Medium;
        private System.Action<Event> handlerFunction;

        public EvHandler(System.Action<Event> handlerFunc, PriorityRank priority = PriorityRank.Default)
        {
            this.handlerFunction = handlerFunc;
            this.Priority = (int)priority;
        }

        public EvHandler(System.Action<Event> handlerFunc, int priority)
        {
            this.handlerFunction = handlerFunc;
            this.Priority = priority;
        }

        public EvHandler(IEvHandler<Event> evHandler)
        {
            this.handlerFunction = evHandler.Call;
            this.Priority = evHandler.Priority;
        }

        public void Call(Event ev)
        {
            handlerFunction(ev);
        }
    }
}