using Utils.Vector;
using Core;
using System;

namespace Core.Items
{
    public class DroppedItem : Entity
    {
        public Item Item { get; set; }
        public static EntityFactory<DroppedItem> s_factory
            = new EntityFactory<DroppedItem>();
        public override Layer Layer => Layer.DROPPED;

        public DroppedItem() : base()
        {
        }
    }
}