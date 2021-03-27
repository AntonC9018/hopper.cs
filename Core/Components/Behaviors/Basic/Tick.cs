namespace Hopper.Core.Components.Basic
{
    [Chains("Do")]
    [ActivationAlias("Tick")]
    public partial class Tick : IBehavior
    {
        public class Context : ActorContext
        {
        }

        // TODO: void return types of activation should be autointerpreted as true?
        //       or allow void return types.
        public void Activate(Entity actor)
        {
            var ev = new Context { actor = actor };
            Do(ctx);
        }
    }
}