namespace Hopper.Core.Behaviors
{
    public static class Utils
    {
        public static System.Action<ActorEvent> AddHistoryEvent(History.UpdateCode eventCode)
        {
            return e => e.actor.History.Add(e.actor, eventCode);
        }
    }
}