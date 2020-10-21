using System.Collections.Generic;
using Core.Behaviors;
using Core.Stats.Basic;
using Utils;

namespace Core.Targeting
{
    public class AtkTarget : Target
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;

        public override Layer TargetedLayer => Layer.REAL;
        public override Layer SkipLayer => Layer.WALL;

        public override void CalculateTargetedEntity(CommonEvent ev, Cell cell)
        {
            base.CalculateTargetedEntity(ev, cell);

            if (targetEntity != null && targetEntity.Behaviors.Has<Attackable>())
            {
                atkCondition = CalculateCondition(targetEntity, (Attacking.Event)ev);
            }
        }

        public static AtkCondition CalculateCondition(Entity entity, Attacking.Event ev)
        {
            var attackable = entity.Behaviors.Get<Attackable>();
            var attackingEvent = ev;
            return attackable.GetAttackableness(attackingEvent);
        }
    }
}