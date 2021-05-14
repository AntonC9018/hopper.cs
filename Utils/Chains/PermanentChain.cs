using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class SelfFilteringChain<Context> : DoubleList<System.Func<Context, bool>>
    {
        public SelfFilteringChain()
        {
        }

        public SelfFilteringChain(List<System.Func<Context, bool>> buffer) : base(buffer)
        {
            _primaryBuffer = new List<System.Func<Context, bool>>(buffer);
        }

        public void PassAndFilter(Context ev)
        {
            foreach (var handler in StartFiltering())
            {
                if (handler(ev))
                {
                    AddToSecondaryBuffer(handler);
                }
            }
        }
    }
}