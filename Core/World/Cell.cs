using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core
{
    public struct Cell
    {
        public List<Transform> m_transforms;
        public event System.Action<Transform> EnterEvent;
        public event System.Action<Transform> LeaveEvent;

        public void Init() { m_transforms = new List<Transform>(); }

        public Transform GetFirstTransform()
        {
            return m_transforms[0];
        }

        public Transform GetAnyTransformFromLayer(Layer layer)
        {
            return m_transforms.FindLast(t => t.layer.HasFlag(layer));
        }

        public List<Transform> GetAllFromLayer(Layer layer)
        {
            return m_transforms.Where(t => t.layer.HasFlag(layer));
        }

        public IEnumerable<Transform> GetAllDirectedFromLayer(IntVector2 direction, Layer layer)
        {
            for (int i = m_transforms.Count - 1; i >= 0; i--)
            {
                var t = m_transforms[i];
                if (t.entity.IsDirected() && t.layer.HasFlag(layer) && t.orientation == direction)
                {
                    yield return t;
                }
            }
        }

        public Transform GetUndirectedTransformFromLayer(Layer layer)
        {
            return m_transforms
                .FindLast(t => t.layer.HasFlag(layer) && t.entity.IsDirected() == false);
        }

        // this one looks for the fitting barriers
        public Transform GetDirectedTransformFromLayer(IntVector2 direction, Layer layer)
        {
            return GetAllDirectedFromLayer(direction, layer).FirstOrDefault();
        }

        public bool HasDirectionalBlock(IntVector2 direction, Layer layer)
        {
            var dir = direction;
            foreach (var t in m_transforms)
            {
                if (t.entity.IsDirected() && t.layer.HasFlag(layer))
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

        public void FireEnterEvent(Transform entity)
        {
            EnterEvent?.Invoke(entity);
        }

        public void FireLeaveEvent(Transform entity)
        {
            LeaveEvent?.Invoke(entity);
        }
    }
}