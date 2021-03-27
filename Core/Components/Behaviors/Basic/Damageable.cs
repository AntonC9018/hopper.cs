using Hopper.Core.Stats.Basic;

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

        public bool Activate(Entity entity, int damage)
        {
            health.amount -= damage;
            if (health.amount <= 0)
            {
                // TODO: remove from grid
                return true;
            }
            return false;
        }
    }
}