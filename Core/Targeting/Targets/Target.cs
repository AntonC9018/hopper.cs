using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public class Target
    {
        public Piece initialPiece;
        public IntVector2 direction;
        public Entity targetEntity;

        public virtual void CalculateTargetedEntity(CommonEvent ev, Cell cell)
        {
            if (cell.GetEntityFromLayer(SkipLayer) == null)
            {
                targetEntity = cell.GetEntityFromLayer(TargetedLayer);
            }
        }

        public virtual Layer TargetedLayer
            => Layer.REAL | Layer.MISC | Layer.WALL;
        public virtual Layer SkipLayer => 0;
    }
}