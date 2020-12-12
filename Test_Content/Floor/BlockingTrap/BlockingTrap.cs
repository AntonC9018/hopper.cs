using System.Collections.Generic;
using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public class BlockingTrap : Entity
    {
        public override Layer Layer => Layer.FLOOR;

        public static EntityFactory<BlockingTrap> CreateFactory(EntityFactory<RealBarrier> factory)
        {
            return new EntityFactory<BlockingTrap>()
                .AddBehavior<Attackable>()
                .AddInitListener(trap => trap.ListenCell())
                .RunAtPatching(Registry.StoreForKind(factory));
        }

        public static EntityFactory<RealBarrier> CreateBarrierFactory() =>
            RealBarrier.CreateFactory()
                .AddBehavior<Attackable>()
                .AddBehavior<Damageable>();

        private static Layer TargetedLayer = Layer.REAL;

        [DataMember] private List<RealBarrier> m_barriers;

        private void ListenCell()
        {
            this.GetCell().EnterEvent += BlockOff;
            this.GetCell().LeaveEvent += Unblock;
            this.DieEvent += RemoveAll;
        }

        private void BlockOff(Entity entity)
        {
            if (entity.Layer.IsOfLayer(TargetedLayer))
            {
                if (m_barriers == null)
                {
                    m_barriers = new List<RealBarrier>(4);
                    var barrierFactory = this.GetFactoryKindData<EntityFactory<RealBarrier>>();
                    foreach (var orientation in IntVector2.Zero.OrthogonallyAdjacent)
                    {
                        m_barriers.Add(entity.World.SpawnEntity(barrierFactory, entity.Pos, orientation));
                    }
                }
                // Tinker.Tink(entity, new BlockedData { applicant = this });
            }
        }

        private void Unblock(Entity entity)
        {
            // if (Tinker.IsTinked(entity))
            // {
            //     Tinker.Untink(entity);
            // }
            TryKillBarriers();
        }

        private void TryKillBarriers()
        {
            if (m_barriers != null && this.GetCell().GetUndirectedEntityFromLayer(TargetedLayer) == null)
            {
                KillBarriers();
            }
        }

        private void KillBarriers()
        {
            foreach (var barrier in m_barriers)
            {
                if (barrier.IsDead == false)
                {
                    barrier.Die();
                }
            }
            m_barriers = null;
        }

        private void RemoveAll()
        {
            this.GetCell().EnterEvent -= BlockOff;
            this.GetCell().LeaveEvent -= Unblock;
            if (m_barriers != null)
            {
                KillBarriers();
            }
        }

        // private static void AttackInstead(Attacking.Event ev)
        // {
        //     var store = Tinker.GetStore(ev);
        //     var barrier = store.applicant.m_barriers?.Find(
        //         b => ev.action.direction == b.Orientation);
        //     if (barrier != null && barrier.IsDead == false)
        //     {
        //         ev.targets = new List<Core.Targeting.AtkTarget>();
        //         ev.targets.Add(new Core.Targeting.AtkTarget
        //         {
        //             targetEntity = barrier
        //         });
        //     }
        // }
    }
}