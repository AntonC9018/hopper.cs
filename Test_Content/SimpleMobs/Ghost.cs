using Hopper.Utils.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content
{
    public class Ghost : Entity
    {
        public static Action Action = new CompositeAction(
            new BehaviorAction<Attacking>(),
            new BehaviorAction<Moving>()
        );

        private static Layer m_teleportedLayer = Layer.REAL | Layer.DROPPED | Layer.GOLD;

        private static void Teleport(Attackable.Event ev)
        {
            if (ev.actor.IsDead && ev.atkParams.attacker != null)
            {
                foreach (var ent in ev.actor.GetCell().GetAllFromLayer(m_teleportedLayer))
                {
                    ent.ResetPosInGrid(ev.atkParams.attacker.Pos);
                }
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

        public static Retoucher CreateTeleportAfterAttack() =>
            Retoucher.SingleHandlered(Attackable.Do, Teleport, PriorityRanks.Lowest);

        public static EntityFactory<Ghost> CreateFactory(Retoucher teleportAfterAttack)
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
                .Retouch(teleportAfterAttack);
        }
    }
}