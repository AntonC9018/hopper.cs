
namespace Core.Items
{
    public abstract class Item
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public abstract void BeEquipped(Entity entity);
        public abstract void BeUnequipped(Entity entity);
        public abstract void BeDestroyed(Entity entity);
    }

    public class TinkerItem : Item
    {
        Tinker tinker;

        public TinkerItem(Tinker tinker)
        {
            this.tinker = tinker;
        }

        public override void BeDestroyed(Entity entity)
        {
            entity.Tink(tinker);
        }

        public override void BeEquipped(Entity entity)
        {
            entity.Tink(tinker);
        }

        public override void BeUnequipped(Entity entity)
        {
            entity.Untink(tinker);
            entity.CreateDroppedItem(id);
        }
    }

    public class ModifierItem : Item
    {
        Modifier modifier;

        public ModifierItem(Modifier modifier)
        {
            this.modifier = modifier;
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
            entity.CreateDroppedItem(id);
        }
    }
}