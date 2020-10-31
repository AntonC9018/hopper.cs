using Core;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Test
{
    public class StaticShooting : Shooting
    {

        private Attack m_attack;
        private Push m_push;
        public StaticShooting(
            Layer targetedLayer,
            Layer skipLayer,
            Attack attack,
            Push push,
            bool stopOnFailedAttack)

            : base(targetedLayer, skipLayer, stopOnFailedAttack)
        {
            m_attack = attack;
            m_push = push;
        }

        protected override void ShootSpecific(Entity entity, Action action)
        {
            foreach (var target in GetShootTargets(entity, action))
            {
                if (ShootOnce_StaticStats(entity, target, action.direction) == false)
                {
                    return;
                }
            }
        }

        // returns true if we shall continue
        private bool ShootOnce_StaticStats(Entity attacker, Entity attacked, IntVector2 direction)
        {
            if (attacked != null && attacked.Behaviors.Has<Attackable>())
            {
                var success = ShootAt(attacker, attacked, direction, m_attack);

                if (m_push != null)
                {
                    PushBack(attacked, direction, m_push);
                }

                if (m_stopOnFailedAttack && success == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}