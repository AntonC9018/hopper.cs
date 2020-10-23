using Core.Behaviors;
using Core.Stats.Basic;

namespace Core.Targeting
{
    public enum AtkCondition
    {
        NEVER = 0,
        ALWAYS = 1,
        SKIP = 2,
        IF_NEXT_TO = 3
    }
    
    public class AtkTarget : Target, ITarget<AtkTarget, Attack>
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;

        public void CalculateTargetedEntity(
            TargetEvent<AtkTarget> ev, Cell cell, Layer skipLayer, Layer targetedLayer)
        {
            targetEntity = GetEntityDefault(cell, skipLayer, targetedLayer);
        }

        public void ProcessMeta(Attack attack)
        {
            if (targetEntity != null && targetEntity.Behaviors.Has<Attackable>())
            {
                atkCondition = CalculateCondition(targetEntity, attack);
            }
        }

        public static AtkCondition CalculateCondition(Entity entity, Attack attack)
        {
            var attackable = entity.Behaviors.Get<Attackable>();
            return attackable.GetAttackableness(attack);
        }
    }
}