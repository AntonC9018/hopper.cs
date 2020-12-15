using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public class PoolEntityContentSpec : IContentSpec
    {
        private string m_poolPath;

        public PoolEntityContentSpec(string poolPath)
        {
            m_poolPath = poolPath;
        }

        public IContent CreateContent(PoolContainer pools, KindRegistry registry)
        {
            return pools.GetEntity(m_poolPath, registry);
        }
    }
}