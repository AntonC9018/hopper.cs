using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core
{
    public class Cell : List<Transform>
    {
        public new void Add(Transform transform) 
        {
            Assert.That(!Contains(transform), "Cannot add a transform twice");
            base.Add(transform);
        }

        public bool TryGetAnyFromLayer(Layer layer, out Transform transform)
        {
            transform = GetAnyFromLayer(layer);
            return transform != null;
        }

        public Transform GetAnyFromLayer(Layer layer)
        {
            return this.FindLast(t => layer.HasFlag(t.layer));
        }

        public IEnumerable<Transform> GetAllFromLayer(Layer layer)
        {
            return this.Where(t => layer.HasFlag(t.layer));
        }

        public IEnumerable<Transform> GetAllDirectedFromLayer(IntVector2 direction, Layer layer)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                var t = this[i];
                if (t.entity.IsDirected() && layer.HasFlag(t.layer) && t.orientation == direction)
                {
                    yield return t;
                }
            }
        }

        public IEnumerable<Transform> GetAllUndirectedFromLayer(Layer layer)
        {
            return GetAllFromLayer(layer).Where(t => !t.entity.IsDirected());
        }

        public Transform GetUndirectedFromLayer(Layer layer)
        {
            return this
                .FindLast(t => layer.HasFlag(t.layer) && t.entity.IsDirected() == false);
        }

        // this one looks for the fitting barriers
        public Transform GetDirectedFromLayer(IntVector2 direction, Layer layer)
        {
            return GetAllDirectedFromLayer(direction, layer).FirstOrDefault();
        }

        public bool HasDirectionalBlock(IntVector2 direction, Layer layer)
        {
            var dir = direction;
            foreach (var t in this)
            {
                if (t.entity.IsDirected() && layer.HasFlag(t.layer))
                {
                    // block diagonal movement if corner barriers are present
                    if (t.orientation.x == dir.x)
                    {
                        dir.x = 0;
                    }
                    if (t.orientation.y == dir.y)
                    {
                        dir.y = 0;
                    }
                    if (dir == IntVector2.Zero)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}