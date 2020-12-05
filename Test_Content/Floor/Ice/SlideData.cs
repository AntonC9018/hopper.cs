using Hopper.Core;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class SlideData : StatusData
    {
        public IntVector2 initialDirection;
        public bool didSlide;

        public SlideData()
        {
        }

        public SlideData(IntVector2 initialDirection)
        {
            this.initialDirection = initialDirection;
            this.amount = 1;
            this.didSlide = true;
        }
    }
}