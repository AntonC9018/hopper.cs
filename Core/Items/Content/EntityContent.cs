using System.Runtime.Serialization;

namespace Hopper.Core.Items
{
    [DataContract]
    public class EntityContent : IContent
    {
        [DataMember]
        private IFactory<Entity> factory;

        public EntityContent(IFactory<Entity> factory)
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