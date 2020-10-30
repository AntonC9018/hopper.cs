namespace Core.Items
{
    public class TinkerItem : Item
    {
        private readonly ISlot m_slot;
        public override ISlot Slot => m_slot;
        protected ITinker m_tinker;

        public TinkerItem(ITinker tinker, ISlot slot) : base()
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