using System;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    [Chains("Death")]
    [NoActivation]
    public partial class Damageable : IBehavior
    {
        public class Context 
        {
            public Entity actor;
        }

        [Inject] public Health health;

        [Alias("IsDead")] public bool IsHealthZero() 
        {
            return health.amount == 0;
        }

        [Alias("Die")] public void Die(Entity actor)
        {
            health.amount = 0;
            DieLogic(actor);
        }

        public void DieLogic(Entity actor)
        {
            _DeathChain.Pass(new Context { actor = actor });
            actor.GetTransform().TryRemoveFromGridWithoutEvent();
        }

        [Alias("BeDamaged")] 
        public bool Activate(Entity actor, int damage)
        {
            health.amount -= damage;
            if (health.amount <= 0)
            {
                DieLogic(actor);
                return true;
            }
            return false;
        }

        public void DefaultPreset()
        {
        }
    }
}