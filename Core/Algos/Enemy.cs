using Core.Utils.Vector;
using Chains;
using Core.Behaviors;
using System.Linq;

namespace Core
{
    static partial class Algos
    {
        static bool AskMove(Acting.Event ev)
        {
            var e = ev.actor.Cell.GetAllFromLayer(ev.action.direction, Layer.REAL);
            bool success = false;
            foreach (var entity in e)
            {
                var acting = entity.Behaviors.TryGet<Acting>();

                if (acting != null
                    && acting.b_didAction == false
                    && acting.b_doingAction == false)
                {
                    acting.Activate();
                    success = true;
                }
            }
            return success;
        }

        static bool Iterate(Acting.Event ev)
        {
            bool success = ev.action.Do(ev.actor);

            if (!success)
            {
                bool otherEntitySuccess = AskMove(ev);

                if (otherEntitySuccess)
                    return Iterate(ev);
            }

            return success;
        }

        public static void EnemyAlgo(EventBase _ev)
        {
            var ev = (Acting.Event)_ev;

            var dirs = ev.actor.Behaviors.Get<Sequential>().GetMovs();

            foreach (var dir in dirs)
            {
                // var action = ev.action.Copy();
                ev.action.direction = dir;
                if (Iterate(ev))
                {
                    ev.success = true;
                    return;
                }
            }

            ev.success = false;
        }

    }
}