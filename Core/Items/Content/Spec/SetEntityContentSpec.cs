using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public class SetEntityContentSpec : IContentSpec
    {
        private IFactory<Entity> factory;

        public SetEntityContentSpec(IFactory<Entity> factory)
        {
            this.factory = factory;
        }

        public IContent CreateContent(Pools pools)
        {
            return new EntityContent(factory);
        }
    }
}