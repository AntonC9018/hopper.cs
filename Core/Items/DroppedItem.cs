namespace Hopper.Core.Items
{
    public class DroppedItem : Entity
    {
        public IItem Item { get; set; }
        public override Layer Layer => Layer.DROPPED;

        public static EntityFactory Factory = CreateFactory();
        public static EntityFactory CreateFactory()
        {
            return new EntityFactory();
        }

        public DroppedItem() : base()
        {
        }
    }
}