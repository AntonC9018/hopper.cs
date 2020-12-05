using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class AnonShooting : ShootingShared, IAnyShooting
    {
        private Attack m_attack;
        private Push m_push;

        public AnonShooting(
            Layer targetedLayer,
            Layer skipLayer,
            Attack attack,
            Push push,
            bool stopAfterFirstAttack)

            : base(targetedLayer, skipLayer, stopAfterFirstAttack)
        {
            m_attack = attack;
            m_push = push;
        }

        public ShootingInfo Shoot(Entity entity, Action action)
        {
            return ShootAnon((IWorldSpot)entity, action.direction);
        }

        public ShootingInfo ShootAnon(IWorldSpot spot, IntVector2 direction)
        {
            var info = GetInitialShootInfo(spot, direction);

            foreach (var target in info.attacked_targets)
            {
                ShootOnceAnon(target, direction);
            }

            return info;
        }

        private void ShootOnceAnon(Entity attacked, IntVector2 direction)
        {
            Attacking.TryApplyAttack(attacked, direction, m_attack, null);
            if (m_push != null)
            {
                Attacking.TryApplyPush(attacked, direction, m_push);
            }
        }
    }
}