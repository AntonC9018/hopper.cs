
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Attackableness
    {
        static System.Action<EventBase> _Constant(Attackable.Attackableness attackableness)
        {
            return (EventBase eventBase) =>
            {
                var ev = (Attackable.AttackablenessEvent)eventBase;
                ev.attackableness = attackableness;
            };
        }

        static Retoucher[] ConstantRetouchers
            = new Retoucher[System.Enum.GetNames(typeof(Attackable.Attackableness)).Length];

        public static Retoucher Constant(Attackable.Attackableness attackableness)
        {
            int index = (int)attackableness;
            if (ConstantRetouchers[index] == null)
            {
                ConstantRetouchers[index] = new Retoucher(
                    new ChainDefinition(
                        "attacked:condition",
                        new WeightedEventHandler(_Constant(attackableness)))
                );
            }
            return ConstantRetouchers[index];
        }

    }
}