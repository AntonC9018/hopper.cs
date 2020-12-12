using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.Utils
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