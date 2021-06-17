using Hopper.Core.ActingNS;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components.Basic
{
    public partial class Moving : IBehavior, IStandartActivateable
    {
        [Chain("Check")]  private Chain<Displaceable.Context> _CheckChain;
        [Chain("Should")] private Chain<Displaceable.Context> _ShouldChain;
        [Chain("After")]  private Chain<Displaceable.Context> _AfterChain;


        [Alias("Move")]
        public bool Activate(Entity actor, IntVector2 direction)
        {
            Assert.That(actor.HasDisplaceable());

            var displ   = actor.GetDisplaceable();
            var move    = actor.GetStats().GetLazy(Move.Index);
            var context = displ.MakeContextWithMove(actor, direction, move);

            if (!_CheckChain.PassWithPropagationChecking(context) || !displ.Check(context))
            {
                return false;
            }

            if (_ShouldChain.PassWithPropagationChecking(context))
            {
                displ.DisplaceLogic(context);
                _AfterChain.Pass(context);
            }

            return true;
        }

        public void DefaultPreset()
        {
        }
    }
}