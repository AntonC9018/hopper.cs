using System.Collections.Generic;
using Hopper.Core.Behaviors;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Core.Items
{
    public class ModularShovel : ModularItem, ITargetProvider<Target>
    {
        private ITargetProvider<Target> m_targetProvider;

        public ModularShovel(
            ItemMetadata meta,
            ITargetProvider<Target> targetProvider,
            params IModule[] modules)
            : base(meta, Hopper.Core.Items.Slot.Shovel, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir)
        {
            return m_targetProvider.GetTargets(spot, dir);
        }
    }
}