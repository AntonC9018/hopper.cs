
using System.Linq;
using Chains;
using Core.Behaviors;

namespace Core.Retouchers
{
    public static class Attackableness
    {
        static System.Action<Attackable.AttackablenessEvent> _Constant(AtkCondition attackableness)
        {
            return (Attackable.AttackablenessEvent ev) =>
            {
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
                ConstantRetouchers[index] = Retoucher
                    .SingleHandlered<Attackable.AttackablenessEvent>(
                        Attackable.Condition.TemplatePath, _Constant(attackableness)
                    );
            }
            return ConstantRetouchers[index];
        }

    }
}