
namespace Core.Items
{
    public abstract class Item
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public abstract void BeEquipped(Entity entity);
        public abstract void BeUnequipped(Entity entity);
        public abstract void BeDestroyed(Entity entity);
        public int slot;
    }

    public class TinkerItem : Item
    {
        Tinker tinker;

        public TinkerItem(Tinker tinker, int slot = 0)
        {
            this.tinker = tinker;
            this.slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            entity.Untink(tinker);
        }

        public override void BeEquipped(Entity entity)
        {
            entity.Tink(tinker);
        }

        public override void BeUnequipped(Entity entity)
        {
            entity.Untink(tinker);
            entity.m_world.CreateDroppedItem(this, entity.m_pos);
        }
    }

    public class ModifierItem : Item
    {
        Modifier modifier;

        public ModifierItem(Modifier modifier, int slot = 0)
        {
            this.modifier = modifier;
            this.slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            entity.m_statManager.RemoveModifier(modifier);
        }

        public override void BeEquipped(Entity entity)
        {
            entity.m_statManager.AddModifier(modifier);
        }

        public override void BeUnequipped(Entity entity)
        {
            entity.m_statManager.RemoveModifier(modifier);
            entity.m_world.CreateDroppedItem(this, entity.m_pos);
        }
    }
}