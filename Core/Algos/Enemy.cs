using Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Core
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
                    && acting.DidAction == false
                    && acting.DoingAction == false)
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

            // if movs if null, consider the action succeeding all the time
            if (dirs == null)
            {
                ev.success = true;
                return;
            }

            foreach (var dir in dirs)
            {
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