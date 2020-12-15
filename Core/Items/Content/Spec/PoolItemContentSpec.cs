using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public class PoolItemContentSpec : IContentSpec
    {
        private string m_poolPath;

        public PoolItemContentSpec(string poolPath)
        {
            m_poolPath = poolPath;
        }

        public IContent CreateContent(PoolContainer pools, KindRegistry registry)
        {
            return pools.GetItem(m_poolPath, registry);
        }
    }
}