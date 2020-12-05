using Chains;
using Hopper.Core.Behaviors;

namespace Test.Utils
{
    public static class Handlers
    {
        public static void StopPropagate(EventBase ev)
        {
            ev.propagate = false;
        }
        public static void StopMove(Moving.Event ev)
        {
            ev.propagate = false;
        }
    }
}