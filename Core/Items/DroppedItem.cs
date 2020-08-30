using Vector;
using Core;

namespace Core.Items
{
    public class DroppedItem : Entity
    {
        public Item Item { get; set; }
        public override Layer Layer { get => Layer.DROPPED; }
        public static EntityFactory<DroppedItem> s_factory
            = new EntityFactory<DroppedItem>();

        static DroppedItem()
        {
            // s_factory.AddBehavior(
        }
    }
}