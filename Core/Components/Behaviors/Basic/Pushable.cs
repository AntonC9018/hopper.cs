using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Stats;

namespace Hopper.Core.Components.Basic
{
    [AutoActivation("Push")]
    public partial class Pushable : IBehavior
    {
        public class Context : ActorContext
        {
            public Push push;
            public IntVector2 direction;
            [Omit] public Push.Resistance resistance;
        }

        [Export] public static void SetResistance(StatManager stats, out Push.Resistance resistance)
        {
            resistance = stats.GetLazy(Push.Resistance.Path);
        }

        [Export] public static void ResistSource(StatManager stats, Push push)
        {
            var sourceRes = stats.GetLazy(Push.Source.Resistance.Path);
            if (sourceRes[push.sourceId] > push.power)
            {
                push.distance = 0;
            }
        }

        [Export] public static void Armor(Context ctx)
        {
            if (ctx.push.pierce <= ctx.resistance.pierce)
            {
                // ctx.propagate = false;
            }
        }

        [Export] public static void BePushed(Entity actor, IntVector2 direction, Push push)
        {
            if (push.distance > 0)
            {
                actor.Displace(direction, push.ConvertToMove());
            }
        }

        // Check { SetResistance, ResistSource, Armor }
        // Do    { BePushed, AddHistoryEvent }
    }
}