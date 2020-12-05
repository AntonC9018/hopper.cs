using Chains;

namespace Hopper.Core
{
    public class ActorEvent : EventBase
    {
        public Entity actor;
    }
    public class StandartEvent : ActorEvent
    {
        public Action action;
    }
}