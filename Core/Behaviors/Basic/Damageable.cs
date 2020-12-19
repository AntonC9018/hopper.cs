using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Damageable : Behavior, IInitable
    {
        public Health m_health;

        public void Init()
        {
            m_entity.InitEvent += () => m_health = m_entity.Stats.GetRawLazy(Health.Path);
        }

        public bool Activate(int damage)
        {
            m_health.amount -= damage;
            if (m_health.amount <= 0)
            {
                m_entity.Die();
                return true;
            }
            return false;
        }

        public static InitableBehaviorFactory<Damageable> Preset
            => new InitableBehaviorFactory<Damageable>(null);
    }
}