using Core;
using Core.Utils.Vector;

namespace Test
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