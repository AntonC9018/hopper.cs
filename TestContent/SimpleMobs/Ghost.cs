using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;
using static Hopper.Core.Action;

namespace Hopper.TestContent
{
    [EntityType]
    public static partial class Ghost
    {
        public static EntityFactory Factory;

        public static readonly CompositeAction GhostAction = Compose(
            FromPredictableActivateable(Attacking.Index),
            FromActivateable(Moving.Index));
        private const Layer TeleportedLayer = Layer.REAL | Layer.DROPPED | Layer.ITEM;


        [Export(Chain = "Attackable.Do", Priority = PriorityRank.Lowest, Dynamic = true)]
        public static void Teleport(Attackable.Context ctx)
        {
            if (ctx.actor.IsDead() && ctx.attacker != null)
            {
                var transform = ctx.actor.GetTransform();
                var attackerTransform = ctx.attacker.GetTransform();

                foreach (var t in transform.GetCell().GetAllFromLayer(TeleportedLayer))
                {
                    t.ResetPositionInGrid(attackerTransform.position);
                }

                attackerTransform.ResetPositionInGrid(transform.position);
            }
        }
        
        public static void AddComponents(Entity subject)
        {
            SequentialMobBase.AddComponents(subject,
                Algos.EnemyAlgo, 
                new Step 
                {
                    action = GhostAction,
                    movs = Movs.Basic
                }
            );
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}