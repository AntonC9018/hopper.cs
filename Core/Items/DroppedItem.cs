namespace Core.Items
{
    public class DroppedItem : Entity
    {
        public IItem Item { get; set; }
        public static EntityFactory<DroppedItem> Factory
            = new EntityFactory<DroppedItem>();
        public override Layer Layer => Layer.DROPPED;

        public DroppedItem() : base()
        {
        }
    }
}