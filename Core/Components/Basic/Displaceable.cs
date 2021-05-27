using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Utils;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Components.Basic
{
    // TODO: Add option for generating linear chains instead of normal chains
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

            public Context(IntVector2 direction, Move move, Transform transform, Layer blockLayer)
            {
                this.direction = direction;
                this.move = move;
                this.transform = transform;
                this.blockLayer = blockLayer;
            }

            public void SetNewPosition()
            {
                int i = 1;

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
                    Propagate = false;
                }
            }

            public void RemoveFromGrid()
            {
                transform.RemoveFromGrid(direction);
                transform.position = newPosition;
            }

            public void ResetInGrid()
            {
                Assert.That(transform.position == newPosition, "Cannot be displaced while displacing");
                transform.ResetInGrid(direction);
            }
        }

        [Chain("Check")] private readonly Chain<Context> _CheckChain;
        [Chain("BeforeRemove")] private readonly Chain<Context> _BeforeRemoveChain;
        [Chain("BeforeReset")] private readonly Chain<Context> _BeforeResetChain;
        [Chain("After")] private readonly Chain<Context> _AfterChain;
        [Inject] public Layer blockLayer;

        // TODO: To support sized entities, a lot has to be done here
        [Alias("Displace")] public bool Activate(Entity actor, IntVector2 direction, Move move)
        {
            var ctx = new Context(direction, move, actor.GetTransform(), blockLayer);

            ctx.SetNewPosition();

            if (!_CheckChain.PassWithPropagationChecking(ctx))
            {
                return false;
            }

            if (_BeforeRemoveChain.PassWithPropagationChecking(ctx))
            {
                ctx.RemoveFromGrid();
                _BeforeResetChain.Pass(ctx);
                ctx.ResetInGrid();
                _AfterChain.Pass(ctx);
            }

            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}
