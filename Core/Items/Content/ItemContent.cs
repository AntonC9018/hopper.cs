using System.Runtime.Serialization;

namespace Hopper.Core.Items
{
    [DataContract]
    public class ItemContent : IContent
    {
        [DataMember]
        private IItem item;

        public ItemContent(IItem item)
        {
            this.item = item;
        }

        private ItemContent()
        {
        }

        public void Release(Entity entity)
        {
            entity.World.SpawnDroppedItem(item, entity.Pos);
        }
    }
}