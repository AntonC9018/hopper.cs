using Hopper.Core;
using Hopper.Core.Behaviors;

namespace Hopper.Test_Content
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

        public static EntityFactory<Water> CreateFactory()
        {
            return new EntityFactory<Water>()
                .AddBehavior<Attackable>()
                .AddInitListener(water => water.ListenCell());
        }

        private Layer m_targetedLayer = Layer.REAL;

        private void ListenCell()
        {
            this.GetCell().EnterEvent += Stuck;
            this.GetCell().LeaveEvent += UnStuck;
            DieEvent += DieHandler;
        }

        private void Stuck(Entity entity)
        {
            if (entity.Layer.IsOfLayer(m_targetedLayer))
            {
                StuckStatus.Status.TryApplyAuto(this, entity);
            }
        }

        private void UnStuck(Entity entity)
        {
            if (StuckStatus.Status.IsApplied(entity))
            {
                StuckStatus.Status.Tinker.GetStore(entity).amount = 0;
            }
        }

        private void DieHandler()
        {
            this.GetCell().EnterEvent -= Stuck;
            this.GetCell().LeaveEvent -= UnStuck;
            foreach (var ent in this.GetCell().GetAllFromLayer(m_targetedLayer))
            {
                UnStuck(ent);
            }
        }
    }
}