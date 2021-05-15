using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Chains;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent
{
    public static partial class Test1
    {
        public class Context : ActorContext {}
        [Chain("Hello")] public static readonly Index<Chain<Context>> Hello = new Index<Chain<Context>>();
    }

    public static partial class Test2
    {
        [Export(Chain = "+Test1.Hello", Dynamic = true)]
        public static void SomeFunc(Entity actor)
        {
        }

        public static void Call(Entity actor)
        {
            var chain = actor.GetMoreChains().GetLazy(Test1.Hello);
            chain.Pass(new Test1.Context { actor = actor });
        }

        public static void Call2(Entity actor)
        {
            var chain = Test1.HelloPath.Follow(actor);
            chain.Pass(new Test1.Context { actor = actor });
        }
    }
}