using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public class PoolEntityContentSpec : IContentSpec
    {
        private string m_poolPath;

        public PoolEntityContentSpec(string poolPath)
        {
            m_poolPath = poolPath;
        }

        public IContent CreateContent(Pools pools)
        {
            return pools.GetEntity(m_poolPath);
        }
    }
}