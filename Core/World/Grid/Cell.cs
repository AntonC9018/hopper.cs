using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core.WorldNS
{
    public class Cell : List<Transform>
    {
        public new void Add(Transform transform) 
        {
            Assert.That(!Contains(transform), "Cannot add a transform twice");
            base.Add(transform);
        }

        public bool TryGetAnyFromLayer(Layers layer, out Transform transform)
        {
            transform = GetAnyFromLayer(layer);
            return transform != null;
        }

        public Transform GetAnyFromLayer(Layers layer)
        {
            return this.FindLast(t => t.layer.HasEitherFlag(layer));
        }

        public IEnumerable<Transform> GetAllFromLayer(Layers layer)
        {
            return this.Where(t => t.layer.HasEitherFlag(layer));
        }

        public IEnumerable<Transform> GetAllDirectedFromLayer(IntVector2 direction, Layers layer)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                var t = this[i];
                if (t.IsDirected() && t.layer.HasEitherFlag(layer) && t.orientation == direction)
                {
                    yield return t;
                }
            }
        }

        public IEnumerable<Transform> GetAllUndirectedFromLayer(Layers layer)
        {
            return GetAllFromLayer(layer).Where(t => !t.IsDirected());
        }

        public Transform GetUndirectedFromLayer(Layers layer)
        {
            return this
                .FindLast(t => t.layer.HasEitherFlag(layer) && t.IsDirected() == false);
        }

        // this one looks for the fitting barriers
        public Transform GetDirectedFromLayer(IntVector2 direction, Layers layer)
        {
            return GetAllDirectedFromLayer(direction, layer).FirstOrDefault();
        }

        public bool HasDirectionalBlock(IntVector2 direction, Layers layer)
        {
            var dir = direction;
            foreach (var t in this)
            {
                if (t.IsDirected() && t.layer.HasEitherFlag(layer))
                {
                    // block diagonal movement if corner barriers are present
                    if (t.orientation.x == dir.x)
                    {
                        dir = new IntVector2(0, dir.y);
                    }
                    if (t.orientation.y == dir.y)
                    {
                        dir = new IntVector2(dir.x, 0);
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