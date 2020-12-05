using Hopper.Utils.Chains;
using Hopper.Core.Behaviors;

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