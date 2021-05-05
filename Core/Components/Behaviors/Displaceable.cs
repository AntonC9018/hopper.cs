using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    // TODO: Add option for generating linear chains instead of normal chains
    [ActivationAlias("Displace")]
    [Chains("Check", "BeforeRemove", "BeforeReset", "After")]
    public partial class Displaceable : IBehavior
    {
        public class Context : ContextBase
        {
            public IntVector2 direction;
            public Move move;
            public Entity actor => transform.entity;
            [Omit] public Transform transform;
            [Omit] public IntVector2 newPosition;
            [Omit] public Layer blockLayer;

            public void SetNewPosition()
            {
                int i = 1;

                var transform = actor.GetTransform();

                do
                {
                    if (transform.HasBlockRelative(direction * i, blockLayer))
                        break;
                    i++;
                } while (i < move.power);
                i--;

                newPosition = transform.GetRelativePosition(direction * i);

                // @Incomplete in this case you should probably add the bump to the history and stop
                // also this should be done in the do chain
                // the thing is that 0 movement messes up some systems of the game
                // e.g. listeners on cell's enter and leave events. 
                if (newPosition == transform.position)
                {
                    propagate = false;
                }
            }

            public void RemoveFromGrid()
            {
                transform.RemoveFromGrid(direction);
                transform.position = newPosition;
            }

            public void ResetInGrid()
            {
                transform.ResetInGrid(direction);
            }
        }

        /* [PassToContext] */ [Inject] public Layer blockLayer;

        // TODO: To support sized entities, a lot has to be done here
        public bool Activate(Entity actor, IntVector2 direction, Move move)
        {
            var ctx = new Context
            {
                move = move,
                blockLayer = blockLayer,
                transform = actor.GetTransform(),
                direction = direction
            };

            ctx.SetNewPosition();

            if (!TraverseCheck(ctx))
            {
                return false;
            }

            if (TraverseBeforeRemove(ctx))
            {
                ctx.RemoveFromGrid();
                TraverseBeforeReset(ctx);
                ctx.ResetInGrid();
                TraverseAfter(ctx);
            }

            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}