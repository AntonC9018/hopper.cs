using Hopper.Core;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public class SlideData : StatusData
    {
        public IntVector2 currentDirection;
        public bool didSlide;

        public SlideData()
        {
        }

        public SlideData(IntVector2 initialDirection)
        {
            this.currentDirection = initialDirection;
            this.amount = 1;
            this.didSlide = true;
        }
    }
}