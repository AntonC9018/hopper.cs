namespace Core.Items
{
    public class ModularItem : IItem
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;
        private IModule[] m_modules;

        public ModularItem(int slot, params IModule[] modules)
        {
            m_modules = modules;
            m_slot = slot;
            m_id = IdMap.Items.Add(this);
        }

        public void BeDestroyed(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeDestroyed(entity);
            }
        }

        public void BeEquipped(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeEquipped(entity);
            }
        }

        public void BeUnequipped(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeUnequipped(entity);
            }
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}