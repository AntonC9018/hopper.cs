using System;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    [Chains("Do")]
    [ActivationAlias("Tick")]
    public partial class Ticking : IBehavior
    {
        public class Context : ActorContext
        {
        }

        // TODO: void return types of activation should be autointerpreted as true?
        //       or allow void return types.
        public bool Activate(Entity actor)
        {
            var ctx = new Context { actor = actor };
            TraverseDo(ctx);
            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}