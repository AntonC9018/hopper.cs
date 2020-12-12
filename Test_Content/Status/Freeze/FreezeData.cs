using Hopper.Core;

namespace Hopper.Test_Content.Status.Freeze
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