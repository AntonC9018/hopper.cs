using Core.Targeting;

namespace Core.Items
{
    public class Item : IItem
    {
        private readonly int m_id;
        public virtual int Slot => throw new System.NotImplementedException();
        public virtual int Id => m_id;

        public Item()
        {
            m_id = IdMap.Items.Add(this);
        }

        public virtual void BeDestroyed(Entity entity)
        {
        }

        public virtual void BeEquipped(Entity entity)
        {
        }

        protected void CreateDropped(Entity entity)
        {
            entity.World.CreateDroppedItem(this, entity.Pos);
        }

        // By default, destroy + drop
        public virtual void BeUnequipped(Entity entity)
        {
            BeDestroyed(entity);
            CreateDropped(entity);
        }

        public static ModularTargetingItem<T, M> CreateModularTargeting<T, M>(int slot,
            IProvideTargets<T, M> targetProvider,
            params IModule[] modules)
                where T : Target, new()
        {
            return new ModularTargetingItem<T, M>(slot, targetProvider, modules);
        }

        public virtual DecomposedItem Decompose() => new DecomposedItem(this);
    }
}