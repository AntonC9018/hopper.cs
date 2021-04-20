using Hopper.Core.Stat.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using System;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("BePushed")]
    public partial class Pushable : IBehavior
    {
        public class Context : ActorContext
        {
            public Push push;
            public IntVector2 direction;
            [Omit] public Push.Resistance resistance;
        }

        [Export] public static void SetResistance(Stats stats, out Push.Resistance resistance)
        {
            stats.GetLazy(Push.Resistance.Index, out resistance);
        }

        [Export] public static void ResistSource(Stats stats, Push push)
        {
            stats.GetLazy(Push.Source.Basic.Index, out var sourceRes);
            if (sourceRes.amount > push.power)
            {
                push.distance = 0;
            }
        }

        [Export] public static void Armor(Context ctx)
        {
            if (ctx.push.pierce <= ctx.resistance.pierce)
            {
                // ??
                // ctx.propagate = false;
            }
        }

        [Export] public static void BePushed(Entity actor, IntVector2 direction, in Push push)
        {
            if (push.distance > 0)
            {
                push.ToMove(out var move);
                actor.Displace(direction, move);
            }
        }

        public void DefaultPreset()
        {
            _CheckChain.Add(SetResistanceHandler, ResistSourceHandler, ArmorHandler);
            _DoChain.Add(BePushedHandler);
        }

        // Check { SetResistance, ResistSource, Armor }
        // Do    { BePushed, AddHistoryEvent }
    }
}