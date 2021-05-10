using System.Linq;

namespace Hopper.Utils.Chains
{
    public static class ChainExtensions
    {
        public static bool PassWithPropagationChecking<Context>(this Chain<Context> chain, Context ctx) where Context : IPropagating
        {
            foreach (var handler in chain.ToArray())
            {
                if (!ctx.Propagate)
                    return false;
                handler.handler(ctx);
            }
            return true;
        }

        public static bool PassWithPropagationChecking<Context>(this LinearChain<Context> chain, Context ctx) where Context : IPropagating
        {
            foreach (var handler in chain.ToArray())
            {
                if (!ctx.Propagate)
                    return false;
                handler(ctx);
            }
            return true;
        }
    }
}