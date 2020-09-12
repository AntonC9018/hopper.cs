using Chains;

namespace Core.Behaviors
{
    public static class Utils
    {
        public static System.Action<CommonEvent> AddHistoryEvent(History.EventCode eventCode)
        {
            return e => e.actor.History.Add(e.actor, eventCode);
        }
    }
}