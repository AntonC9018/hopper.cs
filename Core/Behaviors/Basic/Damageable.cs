using System.Runtime.Serialization;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Damageable : Behavior, IInitable
    {
        public int m_health = 5;

        public void Init(int health)
        {
            m_health = health;
        }

        public bool Activate(int damage)
        {
            m_health -= damage;
            if (m_health <= 0)
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