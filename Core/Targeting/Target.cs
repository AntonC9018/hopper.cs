using System;
using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public class Target
    {
        public int pieceIndex;
        public Piece initialPiece;
        public Entity targetEntity;
        public IntVector2 direction;

        public virtual Layer TargetedLayer
            => Layer.REAL | Layer.MISC | Layer.WALL;

        public virtual Layer SkipLayer => 0;

        public virtual void CalculateCondition(CommonEvent ev)
        { }

        // by default, take the entity from the top
        public virtual void CalculateTargetedEntity(Cell cell)
        {
            if (cell.GetEntityFromLayer(SkipLayer) == null)
            {
                targetEntity = cell.GetEntityFromLayer(TargetedLayer);
            }
        }
    }
}