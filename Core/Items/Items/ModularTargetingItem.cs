using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public static class Item
    {
        public static ModularTargetingItem<T, E> CreateModularTargeting<T, E>(int slot,
            IProvideTargets<T, E> targetProvider,
            params IModule[] modules)
                where T : Target, new()
                where E : TargetEvent<T>
        {
            return new ModularTargetingItem<T, E>(slot, targetProvider, modules);
        }
    }

    public class ModularTargetingItem<T, E> : ModularItem, IProvideTargets<T, E>
        where T : Target, new()
        where E : TargetEvent<T>
    {
        private IProvideTargets<T, E> m_targetProvider;

        public ModularTargetingItem(
            int slot,
            IProvideTargets<T, E> targetProvider,
            params IModule[] modules)
        : base(slot, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IEnumerable<T> GetParticularTargets(E targetEvent)
        {
            return m_targetProvider.GetParticularTargets(targetEvent);
        }

        public List<Target> GetTargets(E targetEvent)
        {
            return m_targetProvider.GetTargets(targetEvent);
        }
    }
}