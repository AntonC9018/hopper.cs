namespace Hopper.Core.Items
{
    public class DroppedItem : Entity
    {
        public IItem Item { get; set; }
        public override Layer Layer => Layer.DROPPED;

        public static EntityFactory<DroppedItem> Factory = CreateFactory();
        public static EntityFactory<DroppedItem> CreateFactory()
        {
            return new EntityFactory<DroppedItem>();
        }

        public DroppedItem() : base()
        {
        }
    }
}