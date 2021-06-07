using System;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    public partial class Damageable : IComponent
    {
        [Inject] public Health health;

        [Alias("BeDamaged")] 
        public bool Activate(Entity actor, int damage)
        {
            health.amount -= damage;
            if (health.amount <= 0)
            {
                actor.TryDie();
                return true;
            }
            return false;
        }

        public void DefaultPreset()
        {
        }
    }
}