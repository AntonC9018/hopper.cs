using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class LinearChain<Context> : List<System.Action<Context>>
    {
        public LinearChain(int capacity = 1) : base(capacity)
        {
        }

        public void PassWithoutStop(Context ev)
        {
            foreach (var handler in this)
            {
                handler(ev);
            }
        }
    }
}