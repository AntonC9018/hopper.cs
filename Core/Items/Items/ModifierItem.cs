using Core.Stats;

namespace Core.Items
{
    public class ModifierItem : Item
    {
        private readonly Slot<IItemContainer> m_slot;
        public override ISlot Slot => m_slot;
        private IModifier modifier;

        public ModifierItem(IModifier modifier, Slot<IItemContainer> slot) : base()
        {
            this.modifier = modifier;
            m_slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            modifier.RemoveSelf(entity.Stats);
        }

        public override void BeEquipped(Entity entity)
        {
            modifier.AddSelf(entity.Stats);
        }
    }
}