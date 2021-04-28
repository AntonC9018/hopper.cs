using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.TestContent;

namespace Hopper.TestContent.Floor
{
    public class StuckInWaterEntityModifier
    {

    }

    public class Water : Entity
    {
        public override Layer Layer => Layer.FLOOR;

        public static EntityFactory<Water> Factory = CreateFactory();
        public static EntityFactory<Water> CreateFactory()
        {
            return new EntityFactory<Water>()
                .AddBehavior(Attackable.DefaultPreset)
                .AddInitListener(water => water.ListenCell());
        }

        public void FreezeUp()
        {
            Die();
            World.SpawnEntity(IceFloor.Factory, m_pos);
        }

        private Layer m_targetedLayer = Layer.REAL;

        private void ListenCell()
        {
            this.GetCell().EnterEvent += ApplyStuck;
            this.GetCell().LeaveEvent += RemoveStuck;
            DieEvent += DieHandler;
        }

        private void ApplyStuck(Entity entity)
        {
            if (entity.Layer.IsOfLayer(m_targetedLayer))
            {
                Stuck.Status.TryApplyAuto(this, entity);
            }
        }

        private void RemoveStuck(Entity entity)
        {
            var status = Stuck.Status;
            if (status.IsApplied(entity))
            {
                status.m_tinker.GetStore(entity).amount = 0;
            }
        }

        private void DieHandler()
        {
            this.GetCell().EnterEvent -= ApplyStuck;
            this.GetCell().LeaveEvent -= RemoveStuck;
            foreach (var ent in this.GetCell().GetAllFromLayer(m_targetedLayer))
            {
                RemoveStuck(ent);
            }
        }
    }
}