using Chains;

namespace Core
{
    public class CommonEvent : EventBase
    {
        public Entity actor;
        public Action action;

        // public CommonEvent(Entity actor, Action action)
        // {
        //     this.actor = actor;
        //     this.action = action;
        // }
    }
}