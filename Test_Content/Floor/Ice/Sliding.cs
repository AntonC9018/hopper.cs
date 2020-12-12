using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.History;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public class Sliding : Behavior, IStandartActivateable
    {
        public static Layer TargetedLayer = Layer.REAL;

        public class Config
        {
            public SlideStatus status;
            public Config(SlideStatus status)
            {
                this.status = status;
            }
        }

        private SlideStatus m_status;

        private void Init(Config config)
        {
            m_status = config.status;
            m_entity.InitEvent += (() => m_entity.GetCell().EnterEvent += ApplySliding);
            m_entity.DieEvent += (() => m_entity.GetCell().EnterEvent -= ApplySliding);
        }

        public bool Activate(Action action)
        {
            var entity = m_entity.GetCell().GetUndirectedEntityFromLayer(TargetedLayer);
            if (entity != null)
            {
                ApplySliding(entity);
            }
            return true;
        }

        private void ApplySliding(Entity entity)
        {
            if (ShouldApplySliding(entity))
            {
                // an alternative way of getting the vector of movement
                // probably should be used if the retoucher `Reorient.OnDisplace` has not been applied.
                // IntVector2 posBefore = entity.History.GetStateBefore(UpdateCode.displaced_do).pos;
                // IntVector2 initialDirection = entity.Pos - posBefore;
                IntVector2 initialDirection = entity.Orientation;

                if (initialDirection != IntVector2.Zero)
                {
                    m_status.TryApplyWithInitialData(
                        m_entity, entity, new SlideData(initialDirection));
                }
            }
        }

        private bool ShouldApplySliding(Entity entity)
        {
            return entity.IsOfLayer(TargetedLayer)
                && entity.IsDirected == false
                && IsWayFree(entity)
                && MovedThisTurn(entity);
        }

        public static bool IsWayFree(Entity entity)
        {
            return entity.HasBlockRelative(entity.Orientation) == false;
        }

        private bool MovedThisTurn(Entity entity)
        {
            return entity.History.Updates
                .Find(u => u.updateCode == UpdateCode.displaced_do) != null;
        }
    }
}