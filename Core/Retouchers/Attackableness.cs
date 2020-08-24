
using System.Linq;
using Chains;

namespace Core.Retouchers
{
    public static class Attackableness
    {
        static System.Action<EventBase> _Constant(AtkCondition attackableness)
        {
            return (EventBase eventBase) =>
            {
                var ev = (Attackable.AttackablenessEvent)eventBase;
                ev.attackableness = attackableness;
            };
        }

        static Retoucher[] ConstantRetouchers
            = new Retoucher[System.Enum.GetNames(typeof(AtkCondition)).Length];

        public static Retoucher Constant(AtkCondition attackableness)
        {
            int index = (int)attackableness;
            if (ConstantRetouchers[index] == null)
            {
                ConstantRetouchers[index] = new Retoucher(
                    new ChainDef<CommonEvent>(
                        "attacked:condition",
                        new EvHandler<CommonEvent>(_Constant(attackableness)))
                );
            }
            return ConstantRetouchers[index];
        }

    }
}