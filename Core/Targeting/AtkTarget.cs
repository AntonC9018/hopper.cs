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

        public override void CalculateCondition(CommonEvent ev)
        {
            if (ev.GetType() != typeof(Attacking.Event))
            {
                throw new System.Exception("Expected event type to be Attacking.Event");
            }

            if (targetEntity != null && targetEntity.Behaviors.Has<Attackable>())
            {
                var attackable = targetEntity.Behaviors.Get<Attackable>();
                var attackingEvent = (Attacking.Event)ev;
                atkCondition = attackable.GetAttackableness(attackingEvent);
            }
        }
    }
}