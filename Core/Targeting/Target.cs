using System;
using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public class Target
    {
        public int index;
        public Piece initialPiece;
        public List<Entity> entities;
        public Entity Entity
        {
            get => entities.Count > 0 ? entities[0] : null;
            set => entities = new List<Entity>(1) { value };
        }
        public IntVector2 direction;

        public virtual void CalculateCondition(CommonEvent ev)
        { }

        // by default, take the entity from the top
        public virtual void CalculateTargets(Cell cell, Layer m_targetedLayer)
        {
            entities = new List<Entity>();
            var entity = cell.GetEntityFromLayer(m_targetedLayer);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }
    }
}