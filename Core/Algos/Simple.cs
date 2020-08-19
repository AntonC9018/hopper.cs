using Chains;

namespace Core
{
    static partial class Algos
    {
        public static void SimpleAlgo(EventBase _ev)
        {
            var ev = (Acting.ActingEvent)_ev;

            bool success = ev.action.Do(ev.actor);
            ev.success = success;
        }

    }
}