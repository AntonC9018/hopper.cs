using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public class SetEntityContentSpec : IContentSpec
    {
        private IFactory<Entity> factory;

        public SetEntityContentSpec(IFactory<Entity> factory)
        {
            this.factory = factory;
        }

        public IContent CreateContent(PoolContainer pools, KindRegistry registry)
        {
            return new EntityContent(factory);
        }
    }
}