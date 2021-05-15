using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Chains;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent
{
    public static partial class Test
    {
        public class Context : ActorContext {}
        [Chain("Hello")] public static readonly Index<Chain<Context>> Hello = new Index<Chain<Context>>();
    }
}