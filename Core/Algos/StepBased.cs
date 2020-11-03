using Core.Behaviors;

namespace Core
{
    static partial class Algos
    {
        public static void StepBased(Acting.Event ev)
        {
            ev.actor.Behaviors.Get<Sequential>().ApplyCurrentAlgo(ev);
        }
    }
}