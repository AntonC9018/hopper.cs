namespace Core.Items
{
    public class ModularItem : Item
    {
        private readonly ISlot m_slot;
        public override ISlot Slot => m_slot;
        private IModule[] m_modules;

        public ModularItem(ItemMetadata meta, ISlot slot, params IModule[] modules) : base(meta)
        {
            m_modules = modules;
            m_slot = slot;
            foreach (var module in modules)
            {
                module.Init(this);
            }
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