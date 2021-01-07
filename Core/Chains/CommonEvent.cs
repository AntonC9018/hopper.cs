using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public class ActorEvent : EventBase
    {
        public Entity actor;
    }
    public class StandartEvent : ActorEvent
    {
        public IntVector2 direction;
    }
}