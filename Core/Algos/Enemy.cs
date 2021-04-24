using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public static partial class Algos
    {
        static bool AskMove(Acting acting, IntVector2 direction)
        {
            var transform = acting.actor.GetTransform();
            var targetTransforms = World.Global.grid.GetAllFromLayer(transform.position, direction, Layer.REAL);
            bool success = false;
            foreach (var targetTransform in targetTransforms)
            {
                if (targetTransform.entity.TryGetActing(out var otherActing)
                    && !otherActing._flags.HasFlag(Acting.Flags.DidAction|Acting.Flags.DoingAction))
                {
                    otherActing.Activate();
                    success = true;
                }
            }
            return success;
        }

        static bool Iterate(Acting acting, ParticularDirectedAction action)
        {
            bool success = action.Do(acting);

            if (!success)
            {
                bool otherEntitySuccess = AskMove(acting, action.direction);

                if (otherEntitySuccess)
                {
                    return Iterate(acting, action);
                }
            }

            return success;
        }

        public static void EnemyAlgo(Acting.Context ctx)
        {
            if (ctx.action is ParticularUndirectedAction)
            {
                ctx.action.Do(ctx.acting);
                return;
            }

            var action = (ParticularDirectedAction)ctx.action;

            var dirs = ctx.actor.GetSequential().GetMovs(ctx.actor);

            // if movs if null, consider the action succeeding all the time
            if (dirs == null)
            {
                ctx.success = true;
                return;
            }

            foreach (var dir in dirs)
            {
                action.direction = dir;
                if (Iterate(ctx.acting, action))
                {
                    ctx.success = true;
                    return;
                }
            }

            ctx.success = false;
        }

    }
}