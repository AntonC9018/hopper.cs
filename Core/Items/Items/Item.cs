using Core.Targeting;

namespace Core.Items
{
    public class Item : IItem
    {
        private readonly int m_id;
        public virtual ISlot Slot => throw new System.NotImplementedException();
        public virtual int Id => m_id;

        // Metadata
        // TODO: might be useful to make this `name` a metadata class / struct
        private readonly ItemMetadata m_metadata;
        public ItemMetadata Metadata => m_metadata;

        public Item(ItemMetadata meta)
        {
            m_id = Registry.Default.Items.Add(this);
            m_metadata = meta;
        }

        public virtual void BeDestroyed(Entity entity)
        {
        }

        public virtual void BeEquipped(Entity entity)
        {
        }

        protected void CreateDropped(Entity entity)
        {
            entity.World.SpawnDroppedItem(this, entity.Pos);
        }

        // By default, destroy + drop
        public virtual void BeUnequipped(Entity entity)
        {
            BeDestroyed(entity);
            CreateDropped(entity);
        }

        public virtual DecomposedItem Decompose() => new DecomposedItem(this);

        public void Init(IItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}