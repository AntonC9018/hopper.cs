using System.Collections.Generic;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Targeting;
using Core.Utils.Vector;

namespace Core.Items
{
    public class ModularWeapon : ModularItem, IAtkTargetProvider
    {
        private IAtkTargetProvider m_targetProvider;

        public ModularWeapon(
            ItemMetadata meta,
            IAtkTargetProvider targetProvider,
            params IModule[] modules)
            : base(meta, Core.Items.Slot.Weapon, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IEnumerable<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack)
        {
            return m_targetProvider.GetTargets(spot, dir, attack);
        }
    }

    public class ModularShovel : ModularItem, ITargetProvider<Target>
    {
        private ITargetProvider<Target> m_targetProvider;

        public ModularShovel(
            ItemMetadata meta,
            ITargetProvider<Target> targetProvider,
            params IModule[] modules)
            : base(meta, Core.Items.Slot.Shovel, modules)
        {
            m_targetProvider = targetProvider;
        }

        public IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir)
        {
            return m_targetProvider.GetTargets(spot, dir);
        }
    }
}