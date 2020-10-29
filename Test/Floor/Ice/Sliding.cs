using Core;
using Core.Behaviors;
using Core.History;
using Core.Utils;

namespace Test
{
    public class Sliding : Behavior, IStandartActivateable
    {
        private Layer m_targetedLayer = Layer.REAL;

        private bool IsWayFree(Entity entity)
        {
            return entity.GetCellRelative(entity.Orientation)
                .GetEntityFromLayer(ExtendedLayer.BLOCK) == null;
        }

        private bool MovedThisTurn(Entity entity)
        {
            return entity.History.Updates
                .Find(u => u.updateCode == UpdateCode.move_do) != null;
        }

        public bool Activate(Action action)
        {
            // System.Console.WriteLine("Activating floor");
            var real = m_entity.Cell.GetEntityFromLayer(m_targetedLayer);
            if (real != null)
            {
                if (IsWayFree(real) && MovedThisTurn(real))
                {
                    SlideStatus.Status.TryApply(m_entity, real, new SlideData(real.Orientation));
                }
            }
            return true;
        }
    }
}