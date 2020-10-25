using Core.Stats;

namespace Core.Items
{
    public interface IModule
    {
        void BeEquipped(Entity entity);
        void BeUnequipped(Entity entity);
        void BeDestroyed(Entity entity);
    }

    public class TinkerModule : IModule
    {
        private ITinker tinker;

        public TinkerModule(ITinker tinker)
        {
            this.tinker = tinker;
        }

        public void BeDestroyed(Entity entity)
        {
            tinker.Untink(entity);
        }

        public void BeEquipped(Entity entity)
        {
            tinker.Tink(entity);
        }

        public void BeUnequipped(Entity entity)
        {
            tinker.Untink(entity);
        }
    }

    public class ModifierModule
    {
        private IModifier modifier;

        public ModifierModule(IModifier modifier, int slot)
        {
            this.modifier = modifier;
        }

        public void BeDestroyed(Entity entity)
        {
            modifier.RemoveSelf(entity.Stats);
        }

        public void BeEquipped(Entity entity)
        {
            modifier.AddSelf(entity.Stats);
        }

        public void BeUnequipped(Entity entity)
        {
            modifier.RemoveSelf(entity.Stats);
        }
    }
}