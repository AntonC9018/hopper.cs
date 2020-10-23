using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Damageable : Behavior
    {
        public int m_health = 5;

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