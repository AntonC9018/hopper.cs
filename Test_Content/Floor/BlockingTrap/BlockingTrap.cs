using System.Collections.Generic;
using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public class BlockingTrap : Trap
    {
        [DataMember] private List<RealBarrier> m_barriers;

        public static EntityFactory<BlockingTrap> Factory = CreateFactory();
        public static readonly EntityFactory<RealBarrier> BarrierFactory = CreateBarrierFactory();

        public static EntityFactory<BlockingTrap> CreateFactory()
        {
            return new EntityFactory<BlockingTrap>()
                .AddBehavior(Attackable.DefaultPreset)
                .AddInitListener(trap => trap.ListenCell());
        }

        public static EntityFactory<RealBarrier> CreateBarrierFactory() =>
            RealBarrier.CreateFactory()
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Damageable.Preset);

        private static Layer TargetedLayer = Layer.REAL;

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
        //         b => ev.direction == b.Orientation);
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