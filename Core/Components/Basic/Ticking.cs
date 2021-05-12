using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    [Chains("Do")]
    [NoActivation]
    public partial class Ticking : IBehavior
    {
        public Entity actor;

        public class Context : ActorContext
        {
        }

        public void Init(Entity actor)
        {
            this.actor = actor;
        }

        public bool Activate()
        {
            var ctx = new Context { actor = actor };
            _DoChain.Pass(ctx);
            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}