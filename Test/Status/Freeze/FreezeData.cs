using Hopper.Core;

namespace Test
{
    public class FreezeData : StatusData
    {
        public IceCube outerEntity;

        public FreezeData()
        {
        }

        public FreezeData(IceCube outerEntity)
        {
            this.outerEntity = outerEntity;
        }
    }
}