using System.Collections.Generic;
using Core.Targeting;

namespace Core.Items
{
    public class BasicTargetingItem : IItem, Targeting.IProvideTargets
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;

        public void BeDestroyed(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        public void BeEquipped(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        public void BeUnequipped(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        public List<Target> GetTargets(CommonEvent commonEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}