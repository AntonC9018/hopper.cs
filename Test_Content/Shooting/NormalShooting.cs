using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class NormalShooting : ShootingShared, INormalShooting
    {
        public NormalShooting(
            TargetLayers layers,
            bool stopOnFailedAttack)

            : base(layers, stopOnFailedAttack)
        {
        }

        private void ShootOnceNormal(Entity attacker, Entity attacked, IntVector2 direction)
        {
            if (attacked != null && attacked.Behaviors.Has<Attackable>())
            {
                var target = new Target(attacked, IntVector2.Zero);

                var success = attacker.Behaviors.Get<Attacking>()
                    .Activate(direction, new List<Target>(1) { target });
            }
        }

        public ShootingInfo Shoot(Entity entity, IntVector2 direction)
        {
            var info = GetInitialShootInfo(entity, direction);
            foreach (var target in info.attacked_targets)
            {
                ShootOnceNormal(entity, target, direction);
            }
            return info;
        }
    }
}