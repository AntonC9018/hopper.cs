using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;

namespace Hopper.TestContent.PinningNS
{
    public partial class PinningComponent : IComponent
    {
        public void InitInWorld(Transform transform)
        {
            var entity = transform.entity;
            transform.SubsribeToPermanentEnterEvent(ctx => Enter(entity, ctx));
        }

        public bool Enter(Entity actor, CellMovementContext ctx)
        {
            if (!actor.TryGetPinningComponent(out var pinning) || actor.IsDead())
            {
                return false; // Remove this listener
            }

            if (ctx.actor.HasPinnedEntityModifier() || ctx.HasMoved())
            {
                return true;
            }

            actor.GetStats().GetLazy(Stat.PinStat.Index, out var pinStat);

            if (ctx.actor.CanNotResist(Stat.PinStat.Source, pinStat.power))
            {
                PinnedEntityModifier.AddTo(ctx.actor, pinStat.amount, actor);
                PinnedEntityModifier.Preset(ctx.actor);
            }
            return true;
        }
    }
}