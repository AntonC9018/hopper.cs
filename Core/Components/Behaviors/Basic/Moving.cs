using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Core.Stat.Basic;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Move")]
    public partial class Moving : IBehavior, IStandartActivateable
    {
        public class Context : StandartContext
        {
            [Omit] public Move move;
        }

        [Export] public static void SetBase(Context ctx)
        {
            ctx.actor.GetStats().GetLazy(Move.Index, out ctx.move);
        }

        [Export] public static void Displace(Context ctx)
        {
            ctx.actor.Displace(ctx.direction, ctx.move);
        }

        public void DefaultPreset()
        {
            _CheckChain.Add(SetBaseHandler);
            _DoChain   .Add(DisplaceHandler);
        }

        // Check { SetBase }
        // Do    { Displace }
    }
}