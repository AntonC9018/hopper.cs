using Core.Stats;

namespace Core.Items
{
    public class ModifierItem : IItem
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;
        private IModifier modifier;

        public ModifierItem(IModifier modifier, int slot)
        {
            this.modifier = modifier;
            m_slot = slot;
            m_id = IdMap.Items.Add(this);
        }

        public void BeDestroyed(Entity entity)
        {
            modifier.AddSelf(entity.Stats);
        }

        public void BeEquipped(Entity entity)
        {
            modifier.AddSelf(entity.Stats);
        }

        public void BeUnequipped(Entity entity)
        {
            modifier.RemoveSelf(entity.Stats);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}