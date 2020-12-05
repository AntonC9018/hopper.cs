using Hopper.Core.Behaviors;
using Hopper.Core.Targeting;

namespace Hopper.Core.Retouchers
{
    public static class Attackableness
    {
        static System.Action<Attackable.AttacknessEvent> _Constant(Attackness attackableness)
        {
            return (Attackable.AttacknessEvent ev) => ev.attackness = attackableness;
        }

        static Retoucher[] ConstantRetouchers
            = new Retoucher[System.Enum.GetNames(typeof(Attackness)).Length];

        public static Retoucher Constant(Attackness attackableness)
        {
            int index = (int)attackableness;
            if (ConstantRetouchers[index] == null)
            {
                ConstantRetouchers[index] = Retoucher
                    .SingleHandlered<Attackable.AttacknessEvent>(
                        Attackable.Condition, _Constant(attackableness)
                    );
            }
            return ConstantRetouchers[index];
        }

    }
}