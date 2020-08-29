using Chains;
using Core.Behaviors;

namespace Core
{
    static partial class Algos
    {
        public static void SimpleAlgo(EventBase _ev)
        {
            var ev = (Acting.Event)_ev;
            bool success = ev.action.Do(ev.actor);

            ev.success = success;
        }

    }
}