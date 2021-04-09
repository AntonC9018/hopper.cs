using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core.Components.Basic
{
    // Withe the PassToContext feature, this will work correclty with an autoactivation
    [AutoActivation("Displace")]
    public partial class Displaceable : IBehavior
    {
        public class Context : StandartContext
        {
            public Move move;
            [Omit] public IntVector2 newPos;
            [Omit] public Layer blockLayer;
        }

        /* [PassToContext] */ [Inject] public Layer blockLayer;


        [Export] public static void ConvertFromMove(Context ctx)
        {
            int i = 1;

            var transform = ctx.actor.GetTransformComponent();

            do
            {
                if (transform.HasBlockRelative(ctx.direction * i, ctx.blockLayer))
                    break;
                i++;
            } while (i < ctx.move.power);
            i--;

            ctx.newPos = transform.GetPosRelative(ctx.direction * i);

            // @Incomplete in this case you should probably add the bump to the history and stop
            // also this should be done in the do chain
            // the thing is that 0 movement messes up some systmes of the game
            // e.g. listeners on cell's enter and leave events. 
            if (ctx.newPos == transform.position)
            {
            }
        }

        [Export] public static void DisplaceRemove(
            TransformComponent transform, IntVector2 newPos)
        {
            transform.RemoveFromGrid();
            transform.position = newPos;
        }

        [Export] public static void DisplaceAddBackHandler(
            TransformComponent transform)
        {
            transform.ResetInGrid();
        }

        // Check { ConvertFromMove }
        // Do    { DisplaceRemove, UpdateHistory, DisplaceAddBack }
    }
}