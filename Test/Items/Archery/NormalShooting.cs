using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.Targeting;

namespace Test
{
    public class NormalShooting : Shooting
    {
        public NormalShooting(
            Layer targetedLayer,
            Layer skipLayer,
            bool stopOnFailedAttack)

            : base(targetedLayer, skipLayer, stopOnFailedAttack)
        {
        }

        protected override void ShootSpecific(Entity entity, Action action)
        {
            foreach (var target in GetShootTargets(entity, action))
            {
                if (ShootOnce_NormalAttack(entity, target, action) == false)
                {
                    return;
                }
            }
        }

        protected bool ShootOnce_NormalAttack(Entity attacker, Entity attacked, Action action)
        {
            if (attacked != null && attacked.Behaviors.Has<Attackable>())
            {
                var atkTarget = new AtkTarget();
                atkTarget.targetEntity = attacked;

                var success = attacker.Behaviors.Get<Attacking>()
                    .Activate(action, new List<AtkTarget>(1) { atkTarget });

                if (m_stopOnFailedAttack && success == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}