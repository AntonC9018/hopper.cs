using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Move")]
    public class Moving : IBehavior, IStandartActivateable
    {
        public class Context : StandartContext
        {
            [Omit] public Move move;
        }

        [Export] public static void SetBase(Context ctx)
        {
            if (ctx.move == null)
            {
                ctx.move = ctx.actor.GetStatManager().GetLazy(Move.Path);
            }
        }

        [Export] public static void Displace(Context ctx)
        {
            ctx.actor.Displace(ctx.direction, ctx.move);
        }

        // Check { SetBase }
        // Do    { Displace }
    }
}