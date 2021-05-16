using System;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    public partial class Damageable : IBehavior
    {
        public class Context 
        {
            public Entity actor;
        }

        [Chain("Death")] private readonly Chain<Context> _DeathChain;
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