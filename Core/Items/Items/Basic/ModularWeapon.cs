using System.Collections.Generic;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Core.Items
{
    public class ModularWeapon : ModularItem, IBufferedAtkTargetProvider
    {
        private IBufferedAtkTargetProvider m_targetProvider;

        public ModularWeapon(
            ItemMetadata meta,
            IBufferedAtkTargetProvider targetProvider,
            params IModule[] modules)
            : base(meta, Hopper.Core.Items.Slot.Weapon, modules)
        {
            m_targetProvider = targetProvider;
        }

        public List<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack)
        {
            return m_targetProvider.GetTargets(spot, dir, attack);
        }
    }
}