using Core.Stats;

namespace Core.Items
{
    public interface IModule
    {
        void BeEquipped(Entity entity);
        void BeUnequipped(Entity entity);
        void BeDestroyed(Entity entity);
        void Init(IItem item);
    }

    public class TinkerModule : IModule
    {
        protected ITinker m_tinker;

        public TinkerModule(ITinker tinker)
        {
            m_tinker = tinker;
        }

        public void BeDestroyed(Entity entity)
        {
            m_tinker.Untink(entity);
        }

        public void BeEquipped(Entity entity)
        {
            m_tinker.Tink(entity);
        }

        public void BeUnequipped(Entity entity)
        {
            m_tinker.Untink(entity);
        }

        public virtual void Init(IItem item)
        {
        }
    }

    public class ModifierModule : IModule
    {
        protected IModifier m_modifier;

        public ModifierModule(IModifier modifier, int slot)
        {
            this.m_modifier = modifier;
        }

        public void BeDestroyed(Entity entity)
        {
            m_modifier.RemoveSelf(entity.Stats);
        }

        public void BeEquipped(Entity entity)
        {
            m_modifier.AddSelf(entity.Stats);
        }

        public void BeUnequipped(Entity entity)
        {
            m_modifier.RemoveSelf(entity.Stats);
        }

        public virtual void Init(IItem item)
        {
        }
    }
}