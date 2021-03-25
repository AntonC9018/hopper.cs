using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    [DataContract]
    public class Displaceable : IBehavior
    {
        public class Context : StandartEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
            public Layer blockLayer;
        }

        [Inject] public Layer blockLayer;

        // TODO: automatically assign blockLayer in the generated activation function
        // through an attibute.
        public bool Activate(Entity entity, IntVector2 dir, Move move)
        {
            var ev = new Context
            {
                actor = entity,
                direction = dir,
                move = move,
                blockLayer = blockLayer
            };
            return CheckDoCycle<Context>(ev);
        }

        public static void ConvertFromMove(Context ctx)
        {
            int i = 1;

            var transform = ctx.actor.GetTransform();

            do
            {
                if (ctransform.HasBlockRelative(ctx.direction * i, ctx.blockLayer))
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

        public static void DisplaceRemove = new Handler<Context>
        {
            handler = (Context ev) =>
            {
                ev.actor.RemoveFromGrid();
                ev.actor.Pos = ev.newPos;
            },
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static Handler<Context> DisplaceAddBackHandler = new Handler<Context>
        {
            handler = (Context ev) =>
            {
                ev.actor.ResetInGrid();
            },
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static Handler<Context> UpdateHistoryHandler = new Handler<Context>
        {
            handler = Utils.AddHistoryEvent(History.UpdateCode.displaced_do),
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        // Check { ConvertFromMove }
        // Do    { DisplaceRemove, UpdateHistory, DisplaceAddBack }
    }
}