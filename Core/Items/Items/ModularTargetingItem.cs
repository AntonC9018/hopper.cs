using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public class ModularTargetingItem<T, M> : ModularItem, IProvideTargets<T, M>
        where T : Target, new()
    {
        private IProvideTargets<T, M> m_targetProvider;

        public ModularTargetingItem(
            int slot,
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
}