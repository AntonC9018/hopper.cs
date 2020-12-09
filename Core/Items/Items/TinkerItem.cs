namespace Hopper.Core.Items
{
    public class TinkerItem : Item
    {
        private readonly ISlot<IItemContainer<IItem>> m_slot;
        public override ISlot<IItemContainer<IItem>> Slot => m_slot;
        protected ITinker m_tinker;

        public TinkerItem(ItemMetadata meta, ITinker tinker, ISlot<IItemContainer<IItem>> slot) : base(meta)
        {
            this.m_tinker = tinker;
            m_slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            System.Console.WriteLine("Untinking tinker");
            m_tinker.Untink(entity);
        }

        public override void BeEquipped(Entity entity)
        {
            System.Console.WriteLine("Tinking tinker");
            m_tinker.Tink(entity);
        }
    }
}