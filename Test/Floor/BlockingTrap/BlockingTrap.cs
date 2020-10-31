using System.Collections.Generic;
using System.Runtime.Serialization;
using Core;
using Core.Behaviors;
using Core.Utils.Vector;

namespace Test
{
    public class BlockingTrap : Entity
    {
        public override Layer Layer => Layer.FLOOR;

        public static EntityFactory<BlockingTrap> CreateFactory()
        {
            return new EntityFactory<BlockingTrap>()
                .AddBehavior<Attackable>()
                .AddInitListener(trap => trap.ListenCell());
        }

        // These cannot be attacked on their own currently.
        // In order to fix this, I will have to introduce a special
        // function for the cell, which will also take a direction
        // and return the barriers if their layer is in the query
        public static EntityFactory<RealBarrier> BarrierFactory =
            RealBarrier.CreateFactory()
                .AddBehavior<Attackable>()
                .AddBehavior<Damageable>();

        private static Layer TargetedLayer = Layer.REAL;
        [DataMember] private List<RealBarrier> m_barriers;
        // private static Tinker<BlockedData> Tinker = Tinker<BlockedData>.SingleHandlered(
        //     // TODO: also, once the entity dies, call TryKillBarriers()
        //     Attacking.Check, AttackInstead, Chains.PriorityRanks.High
        // );

        private void ListenCell()
        {
            Cell.EnterEvent += BlockOff;
            Cell.LeaveEvent += Unblock;
            this.DieEvent += RemoveAll;
        }

        private void BlockOff(Entity entity)
        {
            if (entity.Layer.IsOfLayer(TargetedLayer))
            {
                if (m_barriers == null)
                {
                    m_barriers = new List<RealBarrier>(4);
                    foreach (var orientation in IntVector2.Zero.OrthogonallyAdjacent)
                    {
                        m_barriers.Add(entity.World.SpawnEntity(BarrierFactory, entity.Pos, orientation));
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
            if (m_barriers != null && Cell.GetUndirectedEntityFromLayer(TargetedLayer) == null)
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
            Cell.EnterEvent -= BlockOff;
            Cell.LeaveEvent -= Unblock;
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