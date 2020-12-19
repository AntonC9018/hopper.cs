using Hopper.Utils.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content
{
    public class Ghost : Entity
    {
        public static readonly EntityFactory<Ghost> Factory;
        public static readonly Retoucher TeleportAfterAttackRetoucher;
        public static readonly Action Action;
        private static readonly Layer TeleportedLayer;
        private static readonly Step[] Steps;

        private static void Teleport(Attackable.Event ev)
        {
            if (ev.actor.IsDead && ev.atkParams.attacker != null)
            {
                foreach (var ent in ev.actor.GetCell().GetAllFromLayer(TeleportedLayer))
                {
                    ent.ResetPosInGrid(ev.atkParams.attacker.Pos);
                }
                ev.atkParams.attacker.ResetPosInGrid(ev.actor.Pos);
                // TODO: add update to history
            }
        }

        public static EntityFactory<Ghost> CreateFactory()
        {
            return new EntityFactory<Ghost>()
                .AddBehavior(Moving.Preset)
                .AddBehavior(Displaceable.Preset)
                .AddBehavior(Attacking.Preset)
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Pushable.Preset)
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.EnemyAlgo)))
                .AddBehavior(Sequential.Preset(new Sequential.Config(Steps)))
                .AddBehavior(Damageable.Preset(5))
                .Retouch(TeleportAfterAttackRetoucher);
        }

        static Ghost()
        {
            Action = new CompositeAction(
                new BehaviorAction<Attacking>(),
                new BehaviorAction<Moving>()
            );
            TeleportedLayer = Layer.REAL | Layer.DROPPED | Layer.GOLD;
            Steps = new Step[]
            {
                new Step
                {
                    action = Action,
                    movs = Movs.Basic
                }
            };
            TeleportAfterAttackRetoucher =
               Retoucher.SingleHandlered(Attackable.Do, Teleport, PriorityRanks.Lowest);
            Factory = CreateFactory();
        }
    }
}