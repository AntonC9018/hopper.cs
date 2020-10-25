namespace Core.Items
{
    public class ModularItem : Item
    {
        private readonly int m_slot;
        public override int Slot => m_slot;
        private IModule[] m_modules;

        public ModularItem(int slot, params IModule[] modules) : base()
        {
            m_modules = modules;
            m_slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeDestroyed(entity);
            }
        }

        public override void BeEquipped(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeEquipped(entity);
            }
        }

        public override void BeUnequipped(Entity entity)
        {
            foreach (var mod in m_modules)
            {
                mod.BeUnequipped(entity);
            }
            CreateDropped(entity);
        }
    }
}