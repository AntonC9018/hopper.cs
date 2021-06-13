using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;
using Hopper.Shared.Attributes;
using Hopper.Core.ActingNS;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Retouchers
{
    public static partial class Reorient
    {

        [Export(Chain = "Displaceable.BeforeRemove", Dynamic = true)]
        private static void OnDisplace(IntVector2 direction, Transform transform)
        {
            if (direction != IntVector2.Zero) 
                transform.Reorient(direction);
        }

        [Export(Chain = "Acting.Success", Dynamic = true)]
        private static void OnActionSuccess(CompiledAction action, Transform transform)
        {
            if (action.direction != IntVector2.Zero)
                transform.Reorient(action.direction);
        }

        [Export(Chain = "Acting.Success", Dynamic = true)]
        private static void ToPlayerOnActionSuccess(Transform transform)
        {
            if (transform.TryGetClosestPlayer(out Entity player))
            {
                var diff = player.GetTransform().position - transform.position;
                var sign = diff.Sign();
                var abs = diff.Abs();
                if (abs.x > abs.y)
                {
                    transform.Reorient(new IntVector2(sign.x, 0));
                }
                if (abs.y > abs.x)
                {
                    transform.Reorient(new IntVector2(0, sign.y));
                }
            }
        }
    }
}
