using System.Runtime.Serialization;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    [Chains("Tick")]
    [ActivationAlias("Tick")]
    public class Tick : IBehavior
    {
        public class Context : ActorContext
        {
        }

        // TODO: void return types of activation should be autointerpreted as true?
        //       or allow void return types.
        public void Activate(Entity actor)
        {
            var ev = new Context { actor = actor };
            Tick(ctx);
        }
    }
}