using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.History;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content.Floor
{
    public class Sliding : Behavior
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
            m_entity.InitEvent += (() => m_entity.GetCell().EnterEvent += TryApplySliding);
            m_entity.DieEvent += (() => m_entity.GetCell().EnterEvent -= TryApplySliding);
        }

        private void TryApplySliding(Entity entity)
        {
            System.Console.WriteLine($"ENTITIY ENTERED WITH COORDINSTED {entity.Pos}");
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
    }
}