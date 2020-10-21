using Core.Behaviors;
using Core.Stats.Basic;

namespace Core.Targeting
{
    public class AtkTargetEvent : TargetEvent<AtkTarget>
    {
        public Attack attack;

        public AtkTargetEvent(Attacking.Event ev) : base(ev)
        {
            attack = ev.attack;
        }

        // public AtkTargetEvent(Attack attack, Action action, IWorldPosition entity)
        //     : base(action.direction, entity)
        // {
        //     this.attack = attack;
        // }

        public AtkTargetEvent() { }
    }

    public class AtkTarget : Target, ITarget<AtkTarget, AtkTargetEvent>
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;
        public Layer TargetedLayer => Layer.REAL;
        public Layer SkipLayer => Layer.WALL;

        public void CalculateTargetedEntity(AtkTargetEvent ev, Cell cell)
        {
            targetEntity = GetEntityDefault(cell, SkipLayer, TargetedLayer);

            if (targetEntity != null && targetEntity.Behaviors.Has<Attackable>())
            {
                atkCondition = CalculateCondition(targetEntity, ev);
            }
        }

        public static AtkCondition CalculateCondition(Entity entity, AtkTargetEvent ev)
        {
            var attackable = entity.Behaviors.Get<Attackable>();
            return attackable.GetAttackableness(ev.attack);
        }
    }
}