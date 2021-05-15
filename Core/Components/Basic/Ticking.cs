using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    [NoActivation]
    public partial class Ticking : IBehavior
    {
        // TODO: Linear chain
        [Chain("Do")] private readonly Chain<ActorContext> _DoChain;
        public Entity actor;

        public void Init(Entity actor)
        {
            this.actor = actor;
        }

        public bool Activate()
        {
            var ctx = new ActorContext { actor = actor };
            _DoChain.Pass(ctx);
            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}