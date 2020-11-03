using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.Targeting;
using Core.Utils.Vector;

namespace Test
{
    public class NormalShooting : ShootingShared, INormalShooting
    {
        public NormalShooting(
            Layer targetedLayer,
            Layer skipLayer,
            bool stopOnFailedAttack)

            : base(targetedLayer, skipLayer, stopOnFailedAttack)
        {
        }

        private void ShootOnceNormal(Entity attacker, Entity attacked, Action action)
        {
            if (attacked != null && attacked.Behaviors.Has<Attackable>())
            {
                var atkTarget = new AtkTarget();
                atkTarget.targetEntity = attacked;

                var success = attacker.Behaviors.Get<Attacking>()
                    .Activate(action, new List<AtkTarget>(1) { atkTarget });
            }
        }

        public ShootingInfo Shoot(Entity entity, Action action)
        {
            var info = GetInitialShootInfo(entity, action.direction);
            foreach (var target in info.attacked_targets)
            {
                ShootOnceNormal(entity, target, action);
            }
            return info;
        }
    }
}