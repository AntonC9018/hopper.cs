using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Damageable : Behavior
    {
        public class Config
        {
            public int health;
            public Config(int health)
            {
                this.health = health;
            }
        }

        public int m_health = 5;

        private void Init(Config config)
        {
            if (config != null)
            {
                m_health = config.health;
            }
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
    }
}