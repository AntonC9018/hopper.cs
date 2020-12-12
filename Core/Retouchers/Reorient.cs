
using System.Linq;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Retouchers
{
    public class Reorient
    {
        public Retoucher OnMove = Retoucher
            .SingleHandlered(Moving.Do, AnyReorient, PriorityRanks.High);
        public Retoucher OnDisplace = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public Retoucher OnActionSuccess = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);
        public Retoucher OnAttack = Retoucher
            .SingleHandlered(Displaceable.Do, AnyReorient, PriorityRanks.High);

        public void RegisterAll(Registry registry)
        {
            OnMove.RegisterSelf(registry);
            OnDisplace.RegisterSelf(registry);
            OnActionSuccess.RegisterSelf(registry);
            OnAttack.RegisterSelf(registry);
        }

        private static void AnyReorient(Displaceable.Event ev)
        {
            if (ev.dir != IntVector2.Zero)
            {
                ev.actor.Reorient(ev.dir);
            }
        }
        private static void AnyReorient(StandartEvent ev)
        {
            if (ev.action.direction != IntVector2.Zero)
            {
                ev.actor.Reorient(ev.action.direction);
            }
        }
    }
}