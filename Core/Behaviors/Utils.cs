using Chains;

namespace Core.Behaviors
{
    public static class Utils
    {
        public static System.Action<CommonEvent> AddHistoryEvent(History.EventCode eventCode)
        {
            return e => e.actor.m_history.Add(e.actor, eventCode);
        }
    }
}