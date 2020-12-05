using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Targeting;
using Hopper.Core.Utils.Vector;

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
                var target = new Target(attacked, IntVector2.Zero);

                var success = attacker.Behaviors.Get<Attacking>()
                    .Activate(action, new List<Target>(1) { target });
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