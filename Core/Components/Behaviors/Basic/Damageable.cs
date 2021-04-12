using System;
using Hopper.Core.Stats.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    public partial class Damageable : IComponent
    {
        [Inject] public Health health;

        [Alias("IsDead")] public bool IsHealthZero() 
        {
            return health.amount == 0;
        }

        [Alias("Die")] public void Die()
        {
            health.amount = 0;
        }

        [Alias("BeDamaged")] public bool Activate(Entity actor, int damage)
        {
            health.amount -= damage;
            if (health.amount <= 0)
            {
                // TODO: remove from grid
                return true;
            }
            return false;
        }

        public void DefaultPreset()
        {
        }
    }
}