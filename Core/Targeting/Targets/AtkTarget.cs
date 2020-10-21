using Core.Behaviors;
using Core.Stats.Basic;
using Utils.Vector;

namespace Core.Targeting
{
    public class AtkTarget : Target, ITarget<AtkTarget, Attack>
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;
        public Layer TargetedLayer => Layer.REAL;
        public Layer SkipLayer => Layer.WALL;

        public void CalculateTargetedEntity(TargetEvent<AtkTarget> ev, Cell cell, Attack attack)
        {
            targetEntity = GetEntityDefault(cell, SkipLayer, TargetedLayer);

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