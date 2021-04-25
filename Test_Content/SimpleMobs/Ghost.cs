using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Core.Targeting;
using Hopper.Core.Items;

namespace Hopper.TestContent
{
    [EntityType]
    public static class Ghost
    {
        public static EntityFactory Factory;

        public static readonly Action GhostAction = GhostAction = 
            Action.CreateCompositeDirected(
                Action.CreateFromActivateable(Attacking.Index),
                Action.CreateFromActivateable(Moving.Index));
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
            Equippable.AddToInventoryCountableHandlerWrapper.AddTo(subject);
        }
    }
}