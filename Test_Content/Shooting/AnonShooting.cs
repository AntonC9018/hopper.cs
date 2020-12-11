using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class AnonShooting : ShootingShared, IAnyShooting
    {
        private System.Func<Registry, Attack> m_attackFunc; // TODO: take from function
        private System.Func<Registry, Push> m_pushFunc; // TODO: take from function

        public AnonShooting(
            Layer targetedLayer,
            Layer skipLayer,
            System.Func<Registry, Attack> attack,
            System.Func<Registry, Push> push,
            bool stopAfterFirstAttack)

            : base(targetedLayer, skipLayer, stopAfterFirstAttack)
        {
            m_attackFunc = attack;
            m_pushFunc = push;
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
            Attacking.TryApplyAttack(attacked, direction,
                m_attackFunc(attacked.World.m_currentRegistry), null);

            if (m_pushFunc != null)
            {
                Attacking.TryApplyPush(attacked, direction,
                    m_pushFunc(attacked.World.m_currentRegistry));
            }
        }
    }
}