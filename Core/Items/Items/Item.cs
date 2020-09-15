
using Utils;

namespace Core.Items
{
    public interface IItem : IHaveId
    {
        void BeEquipped(Entity entity);
        void BeUnequipped(Entity entity);
        void BeDestroyed(Entity entity);
        int Slot { get; }
    }
}