using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.TestContent;

namespace Hopper.TestContent.Floor
{
    /*
        This water has been implemented without using behaviors
        It makes use solely of events: InitEvent and DieEvent of the entity
        And the Enter and Leave events of the Cell.
        Currently, retouchers can only make use of the tick chain to check
        whether the entity has died. The two tools, the events and the
        chain, should somehow be united. In the former code, I had the 
        `Killable` behavior, which is not necessarily a bad idea, since
        chains are better than normal events (they have priorities).
    */
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