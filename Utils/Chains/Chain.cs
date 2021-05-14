using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Hopper.Utils.Chains
{
    public sealed class Chain<Context> : SortedSet<Handler<Context>>, IChain<Handler<Context>>
    {
        public Chain() : base()
        {
        }

        public Chain(IEnumerable<Handler<Context>> other) : base(other)
        {
        }

        public void AddMany(params Handler<Context>[] handlers)
        {
            foreach (var handler in handlers)
                Add(handler);
        }

        public void Pass(Context ev)
        {
            foreach (var handler in this.ToArray())
            {
                handler.handler(ev);
            }
        }

        ICopyable ICopyable.Copy() => new Chain<Context>(this);
        public Chain<Context> Copy() => new Chain<Context>(this);
        void IChain<Handler<Context>>.Add(Handler<Context> handler) => Add(handler);
    }
}