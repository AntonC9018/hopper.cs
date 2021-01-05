using Hopper.Utils.Vector;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Core
{
    public static class CellExtensions
    {
        public static bool GetAnyEntityFromLayer(this Cell cell, Layer layer, out Entity entity)
        {
            entity = cell.GetAnyEntityFromLayer(layer);
            return entity != null;
        }

        public static bool GetAllFromLayer(this Cell cell, Layer layer, out List<Entity> entities)
        {
            entities = cell.GetAllFromLayer(layer);
            return entities.Count != 0;
        }

        public static bool GetEntityFromLayer(this Cell cell, IntVector2 direction, Layer layer, out Entity entity)
        {
            entity = cell.GetAllFromLayer(direction, layer).FirstOrDefault();
            return entity != null;
        }
        public static bool GetEntityFromLayerBut(this Cell cell, IntVector2 direction, Layer layer, Entity entityToIgnore, out Entity entity)
        {
            var entities = cell.GetAllFromLayer(direction, layer);
            entity = null;
            foreach (var e in entities)
            {
                if (e != entityToIgnore)
                {
                    entity = e;
                    break;
                }
            }
            return entity != null;
        }

        // this one looks for the fitting barriers
        public static bool GetDirectedEntityFromLayer(this Cell cell, IntVector2 direction, Layer layer, out Entity entity)
        {
            entity = cell.GetAllDirectedFromLayer(direction, layer).FirstOrDefault();
            return entity != null;
        }

        public static bool GetUndirectedEntityFromLayer(this Cell cell, Layer layer, out Entity entity)
        {
            entity = cell.m_entities.FindLast(e => e.IsOfLayer(layer) && e.IsDirected == false);
            return entity != null;
        }
    }
}