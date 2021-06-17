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
            public Entity actor => transform.entity;
            public Transform transform;
            public IntVector2 initialPosition;
            public IntVector2 newPosition;
            public Layers blockLayer;

            public Context(Transform transform, IntVector2 direction)
            {
                this.direction       = direction;
                this.transform       = transform;
                this.initialPosition = transform.position;
            }

            public void SetNewPosition(Move move, Layers blockLayer)
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

            /// <summary>
            /// Set the next position to the given value.
            /// This would make the entity teleport if followed by DisplaceLogic().
            /// </summary>
            public void SetNewPosition(IntVector2 position)
            {
                newPosition = position;
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

        [Chain("Check")]        private readonly Chain<Context> _CheckChain;
        [Chain("BeforeRemove")] private readonly Chain<Context> _BeforeRemoveChain;
        [Chain("BeforeReset")]  private readonly Chain<Context> _BeforeResetChain;
        [Chain("After")]        private readonly Chain<Context> _AfterChain;
        [Inject] public Layers blockLayer;

        // TODO: To support sized entities, a lot has to be done here
        [Alias("Displace")] 
        public bool Activate(Entity actor, IntVector2 direction, Move move)
        {
            var ctx = MakeContextWithMove(actor, direction, move);

            if (!Check(ctx))
            {
                return false;
            }

            DisplaceLogic(ctx);
            return true;
        }

        [Alias("DisplaceTo")] 
        public void Activate(Entity actor, IntVector2 direction, IntVector2 newPosition)
        {
            var ctx = new Context(actor.GetTransform(), direction);
            ctx.SetNewPosition(newPosition);
            DisplaceLogic(ctx);
        }

        public Context MakeContextWithMove(Entity actor, IntVector2 direction, Move move)
        {
            var ctx = new Context(actor.GetTransform(), direction);
            ctx.SetNewPosition(move, blockLayer);
            return ctx;
        }

        public bool Check(Context context)
        {
            return _CheckChain.PassWithPropagationChecking(context);
        }

        public void DisplaceLogic(Context context)
        {
            _BeforeRemoveChain.Pass(context);
            context.RemoveFromGrid();
            _BeforeResetChain.Pass(context);
            context.ResetInGrid();
            _AfterChain.Pass(context);
        }

        public void DefaultPreset()
        {
        }
    }
}
