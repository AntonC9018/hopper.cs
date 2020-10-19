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
        private Modifier modifier;

        public ModifierModule(Modifier modifier, int slot)
        {
            this.modifier = modifier;
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
        }
    }
}