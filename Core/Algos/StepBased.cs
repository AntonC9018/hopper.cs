using Hopper.Core.Behaviors.Basic;

namespace Hopper.Core
{
    static partial class Algos
    {
        public static void StepBased(Acting.Event ev)
        {
            ev.actor.Behaviors.Get<Sequential>().ApplyCurrentAlgo(ev);
        }
    }
}