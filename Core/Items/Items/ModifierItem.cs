using Hopper.Core.Stats;

namespace Hopper.Core.Items
{
    public class ModifierItem : Item
    {
        private readonly Slot<IItemContainer, IItem> m_slot;
        public override ISlot<IItem> Slot => m_slot;
        private IModifier modifier;

        public ModifierItem(ItemMetadata meta, IModifier modifier, Slot<IItemContainer, IItem> slot)
            : base(meta)
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