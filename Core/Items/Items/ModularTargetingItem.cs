using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public class ModularTargetingItem : ModularItem, IProvideTargets
    {
        private IProvideTargets m_targetProvider;
        public ModularTargetingItem(
            int slot,
            IProvideTargets targetProvider,
            params IModule[] modules)
        : base(slot, modules)
        {
            m_targetProvider = targetProvider;
        }

        public List<Target> GetTargets(CommonEvent commonEvent)
        {
            return m_targetProvider.GetTargets(commonEvent);
        }
    }
}