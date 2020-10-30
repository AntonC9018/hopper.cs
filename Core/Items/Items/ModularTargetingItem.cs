using System.Collections.Generic;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Targeting;

namespace Core.Items
{
    public class ModularTargetingItem<T, M> : ModularItem, IProvideTargets<T, M>
        where T : Target, new()
    {
        private IProvideTargets<T, M> m_targetProvider;

        public ModularTargetingItem(
            ISlot slot,
            IProvideTargets<T, M> targetProvider,
            params IModule[] modules)
        : base(slot, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IEnumerable<T> GetParticularTargets(TargetEvent<T> targetEvent, M meta)
        {
            return m_targetProvider.GetParticularTargets(targetEvent, meta);
        }

        public List<Target> GetTargets(TargetEvent<T> targetEvent, M meta)
        {
            return m_targetProvider.GetTargets(targetEvent, meta);
        }
    }

    public class ModularWeapon : ModularTargetingItem<AtkTarget, Attackable.Params>
    {
        public ModularWeapon(IProvideTargets<AtkTarget, Attackable.Params> targetProvider, params IModule[] modules)
            : base(Core.Items.Slot.Weapon, targetProvider, modules)
        {
        }
    }

    public class ModularShovel : ModularTargetingItem<DigTarget, Dig>
    {
        public ModularShovel(IProvideTargets<DigTarget, Dig> targetProvider, params IModule[] modules)
            : base(Core.Items.Slot.Shovel, targetProvider, modules)
        {
        }
    }
}