using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;

namespace Hopper.TestContent.Floor
{
    // Put on ice or whatever
    public partial class SlipperyComponent : IComponent
    {
        public static Layer TargetedLayer = Layer.REAL;

        public void InitInWorld(Transform transform)
        {
            transform.SubsribeToPermanentEnterEvent(ctx => Enter(transform.entity, ctx));
        }

        public static bool Enter(Entity actor, CellMovementContext context)
        {
            if (!actor.TryGetSlipperyComponent(out var slippery) || actor.IsDead())
            {
                return false;
            }

            if (context.transform.layer.HasFlag(TargetedLayer) && context.HasNotMoved())
            {
                SlidingEntityModifier.TryApplyTo(context.transform.entity, context.transform.orientation);
            }

            return true;
        }
    }
}