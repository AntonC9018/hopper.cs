using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Core.Items
{
    public class ModularShovel : ModularItem, ITargetProvider
    {
        private ITargetProvider m_targetProvider;

        public ModularShovel(
            ItemMetadata meta,
            ITargetProvider targetProvider,
            params IModule[] modules)
            : base(meta, Hopper.Core.Items.BasicSlots.Shovel, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IPattern Pattern => m_targetProvider.Pattern;

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir)
        {
            return m_targetProvider.GetTargets(spot, dir);
        }
    }
}