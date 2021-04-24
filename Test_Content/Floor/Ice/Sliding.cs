using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.History;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Floor
{
    public class Sliding : IBehavior<SlideStatus>
    {
        public static Layer TargetedLayer = Layer.REAL;

        private SlideStatus m_status;

        public void Init(SlideStatus status)
        {
            m_status = status;
            m_entity.InitEvent += (() => m_entity.GetCell().EnterEvent += TryApplySliding);
            m_entity.DieEvent += (() => m_entity.GetCell().EnterEvent -= TryApplySliding);
        }

        private void TryApplySliding(Entity entity)
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
                && IsWayFree(entity);
        }

        public static bool IsWayFree(Entity entity)
        {
            return entity.HasBlockRelative(entity.Orientation) == false;
        }

        public static ConfigurableBehaviorFactory<Sliding, SlideStatus> Preset(SlideStatus slideStatus)
            => new ConfigurableBehaviorFactory<Sliding, SlideStatus>(null, slideStatus);
    }
}