using System.Collections.Generic;
using Core.Behaviors;
using Utils;

namespace Core.Targeting
{
    public class AtkTarget : Target
    {
        public AtkCondition atkCondition;

        public override void CalculateCondition(CommonEvent ev)
        {
            if (ev.GetType() != typeof(Attacking.Event))
            {
                throw new System.Exception("Expected event type to be Attacking.Event");
            }
            if (Entity != null)
            {
                var attackable = Entity.Behaviors.Get<Attackable>();

                atkCondition =
                    attackable == null
                        ? AtkCondition.NEVER
                        // TODO: this requires action with the attack already set
                        : attackable.GetAttackableness((Attacking.Event)ev);
            }
            else
            {
                atkCondition = AtkCondition.NEVER;
            }
        }

        public override void CalculateTargets(Cell cell, Layer m_targetedLayer)
        {
            // I do not know what exactly to do here
            entities = cell.m_entities.Where(e => (e.Layer & m_targetedLayer) != 0);
        }

    }
}