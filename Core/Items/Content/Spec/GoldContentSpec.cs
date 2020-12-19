using Hopper.Core.Registries;

namespace Hopper.Core.Items
{
    public class GoldContentSpec : IContentSpec
    {
        private int amount;

        public GoldContentSpec(int amount)
        {
            this.amount = amount;
        }

        public IContent CreateContent(Pools pools)
        {
            throw new System.NotImplementedException();
        }
    }
}