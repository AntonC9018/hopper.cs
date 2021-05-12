using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class LinearChain<Context> : List<System.Action<Context>>, IChain
    {
        public LinearChain() : base()
        {
        }

        public LinearChain(IEnumerable<System.Action<Context>> collection) : base(collection)
        {
        }

        public void PassWithoutStop(Context ev)
        {
            foreach (var handler in this)
            {
                handler(ev);
            }
        }

        ICopyable ICopyable.Copy() => new LinearChain<Context>(this);
        public LinearChain<Context> Copy() => new LinearChain<Context>(this);
    }
}