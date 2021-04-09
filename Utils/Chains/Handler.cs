using System;

namespace Hopper.Utils.Chains
{
    public struct Handler<Context>
    {
        public int priority;
        public Action<Context> handler;


        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return priority;
        }

        public static bool operator==(Handler<Context> ctx1, Handler<Context> ctx2)
        {
            return ctx1.priority == ctx2.priority;
        }

        public static bool operator!=(Handler<Context> ctx1, Handler<Context> ctx2)
        {
            return ctx1.priority != ctx2.priority;
        }
    }

}
