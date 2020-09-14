
using Utils;

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
}