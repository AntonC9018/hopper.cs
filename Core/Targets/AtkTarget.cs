using Core.Behaviors;

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
            if (entity != null)
            {
                var attackable = entity.GetBehavior<Attackable>();

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
    }
}