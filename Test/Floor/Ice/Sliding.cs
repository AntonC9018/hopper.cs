using Core;
using Core.Behaviors;
using Core.History;
using Core.Utils;
using Core.Utils.Vector;

namespace Test
{
    public class Sliding : Behavior, IStandartActivateable
    {
        public static Layer TargetedLayer = Layer.REAL;

        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_entity = entity;
            entity.InitEvent += (() => m_entity.Cell.EnterEvent += ApplySliding);
            entity.DieEvent += (() => m_entity.Cell.EnterEvent -= ApplySliding);
        }

        public bool Activate(Action action)
        {
            var real = m_entity.Cell.GetEntityFromLayer(TargetedLayer);
            if (real != null)
            {
                ApplySliding(real);
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
                    SlideStatus.Status.TryApply(
                        m_entity, entity, new SlideData(initialDirection));
                }
            }
        }

        private bool ShouldApplySliding(Entity entity)
        {
            return entity.IsOfLayer(TargetedLayer)
                && IsWayFree(entity)
                && MovedThisTurn(entity);
        }

        public static bool IsWayFree(Entity entity)
        {
            return entity.GetCellRelative(entity.Orientation)
                .GetEntityFromLayer(ExtendedLayer.BLOCK) == null;
        }

        private bool MovedThisTurn(Entity entity)
        {
            return entity.History.Updates
                .Find(u => u.updateCode == UpdateCode.displaced_do) != null;
        }
    }
}