using Hopper.Core.Registry;

namespace Hopper.Core.Items
{
    public class GoldContentSpec : IContentSpec
    {
        private int amount;

        public GoldContentSpec(int amount)
        {
            this.amount = amount;
        }

        public IContent CreateContent(PoolContainer pools, Registry.Registry registry)
        {
            throw new System.NotImplementedException();
        }
    }
}