using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public static partial class Algos
    {
        static bool AskMove(Entity actor, IntVector2 direction)
        {
            var e = actor.GetCell().GetAllFromLayer(direction, Layer.REAL);
            bool success = false;
            foreach (var entity in e)
            {
                var acting = entity.Behaviors.TryGet<Acting>();

                if (acting != null
                    && acting.HasDoneAction() == false
                    && acting.IsDoingAction() => == false)
                {
                    acting.Activate();
                    success = true;
                }
            }
            return success;
        }

        static bool Iterate(Entity actor, ParticularDirectedAction action)
        {
            bool success = action.Do(actor);

            if (!success)
            {
                bool otherEntitySuccess = AskMove(actor, action.direction);

                if (otherEntitySuccess)
                {
                    return Iterate(actor, action);
                }
            }

            return success;
        }

        public static void EnemyAlgo(Acting.Event ev)
        {
            if (ev.action is ParticularUndirectedAction)
            {
                ev.action.Do(ev.actor);
                return;
            }

            var action = (ParticularDirectedAction)ev.action;

            var dirs = ev.actor.Behaviors.Get<Sequential>().GetMovs();

            // if movs if null, consider the action succeeding all the time
            if (dirs == null)
            {
                ev.success = true;
                return;
            }

            foreach (var dir in dirs)
            {
                action.direction = dir;
                if (Iterate(ev.actor, action))
                {
                    ev.success = true;
                    return;
                }
            }

            ev.success = false;
        }

    }
}