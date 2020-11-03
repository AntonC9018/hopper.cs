using Chains;
using Core.Behaviors;

namespace Core
{
    static partial class Algos
    {
        public static void SimpleAlgo(Acting.Event ev)
        {
            ev.success = ev.action.Do(ev.actor);
        }

    }
}