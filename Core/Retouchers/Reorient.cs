using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Retouchers
{
    public static partial class Reorient
    {

        [Export(Chain = "Displaceable.Do", Dynamic = true)]
        private static void OnDisplace(IntVector2 direction, TransformComponent transform)
        {
            if (direction != IntVector2.Zero) 
                transform.orientation = direction;
        }

        [Export(Chain = "Acting.Success", Dynamic = true)]
        private static void OnActionSuccess(ParticularAction action, TransformComponent transform)
        {
            if (action is ParticularDirectedAction directedAction)
                transform.orientation = directedAction.direction;
        }

        [Export(Chain = "Acting.Success", Dynamic = true)]
        private static void ToPlayerOnActionSuccess(TransformComponent transform)
        {
            if (transform.TryGetClosestPlayer(out var player))
            {
                var diff = player.GetTransformComponent().position - transform.position;
                var sign = diff.Sign();
                var abs = diff.Abs();
                if (abs.x > abs.y)
                {
                    transform.orientation = new IntVector2(sign.x, 0);
                }
                if (abs.y > abs.x)
                {
                    transform.orientation = new IntVector2(0, sign.y);
                }
            }
        }
    }
}