using Core.Stats;

namespace Core.Items
{
    public class ModifierItem : Item
    {
        private readonly int m_slot;
        public override int Slot => m_slot;
        private IModifier modifier;

        public ModifierItem(IModifier modifier, int slot) : base()
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