using System.Runtime.Serialization;

namespace Core.Items
{
    [DataContract]
    public class EntityContent : IContent
    {
        [DataMember]
        private IEntityFactory factory;

        public EntityContent(IEntityFactory factory)
        {
            this.factory = factory;
        }

        private EntityContent()
        {
        }

        public void Release(Entity entity)
        {
            entity.World.SpawnEntity(factory, entity.Pos);
        }
    }
}