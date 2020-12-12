using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Core.Retouchers
{
    public class AttacknessRetouchers
    {
        private static System.Action<Attackable.AttacknessEvent> _Constant(Attackness attackness)
        {
            return (Attackable.AttacknessEvent ev) => ev.attackness = attackness;
        }

        public void RegisterAll(Registry registry)
        {
            foreach (var r in ConstantRetouchers)
            {
                if (r != null)
                    r.RegisterSelf(registry);
            }
        }

        private Retoucher[] ConstantRetouchers
           = new Retoucher[System.Enum.GetNames(typeof(Attackness)).Length];

        public Retoucher Constant(Attackness attackness)
        {
            int index = (int)attackness;
            if (ConstantRetouchers[index] == null)
            {
                return Retoucher
                    .SingleHandlered<Attackable.AttacknessEvent>(
                        Attackable.Condition, _Constant(attackness)
                    );
            }
            return ConstantRetouchers[index];
        }

    }
}