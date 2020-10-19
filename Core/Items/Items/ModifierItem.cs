using Core.Stats;

namespace Core.Items
{
    public class ModifierItem : IItem
    {
        private readonly int m_id;
        private readonly int m_slot;
        public int Slot => m_slot;
        public int Id => m_id;
        private Modifier modifier;

        public ModifierItem(Modifier modifier, int slot)
        {
            this.modifier = modifier;
            m_slot = slot;
            m_id = IdMap.Items.Add(this);
        }

        public void BeDestroyed(Entity entity)
        {
            entity.Stats.RemoveModifier(modifier);
        }

        public void BeEquipped(Entity entity)
        {
            entity.Stats.AddModifier(modifier);
        }

        public void BeUnequipped(Entity entity)
        {
            entity.Stats.RemoveModifier(modifier);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}