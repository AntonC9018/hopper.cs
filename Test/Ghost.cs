using Chains;
using Core;
using Core.Behaviors;

namespace Test
{
    public class Ghost : Entity
    {
        public static Action Action = new CompositeAction(
            new BehaviorAction<Attacking>(),
            new BehaviorAction<Moving>()
        );

        private static void Teleport(Attackable.Event ev)
        {
            if (ev.actor.IsDead && ev.atkParams.attacker != null)
            {
                ev.atkParams.attacker.ResetPosInGrid(ev.actor.Pos);
                // TODO: add update to history
            }
        }

        private static Step[] steps = new Step[]
        {
            new Step
            {
                action = Action,
                movs = Movs.Basic
            }
        };

        public static Retoucher TeleportAfterAttack =
            Retoucher.SingleHandlered(Attackable.Do, Teleport, PriorityRanks.Lowest);

        public static EntityFactory<Ghost> CreateFactory()
        {
            return new EntityFactory<Ghost>()
                .AddBehavior<Moving>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Attacking>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(steps))
                .AddBehavior<Damageable>()
                .Retouch(TeleportAfterAttack);
        }
    }
}