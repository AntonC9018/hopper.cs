using Hopper.Core.Behaviors.Basic;

namespace Hopper.Core
{
    static partial class Algos
    {
        public static void SimpleAlgo(Acting.Event ev)
        {
            ev.success = ev.action.Do(ev.actor);
        }

    }
}