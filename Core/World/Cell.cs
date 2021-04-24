using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;

namespace Hopper.Core
{
    public struct Cell
    {
        public List<Transform> _transforms;
        public event System.Action<Transform> EnterEvent;
        public event System.Action<Transform> LeaveEvent;

        public void Init() { _transforms = new List<Transform>(); }

        public bool Remove(Transform transform) => _transforms.Remove(transform);
        public bool Contains(Transform transform) => _transforms.Contains(transform);
        public void Add(Transform transform) 
        {
            Assert.That(!_transforms.Contains(transform), "Cannot add a transform twice");
            _transforms.Add(transform);
        }

        public Transform GetFirstTransform()
        {
            return _transforms[0];
        }

        public IEnumerable<Transform> GetAllTransforms() => _transforms;

        public Transform GetAnyTransformFromLayer(Layer layer)
        {
            return _transforms.FindLast(t => t.layer.HasFlag(layer));
        }

        public IEnumerable<Transform> GetAllFromLayer(Layer layer)
        {
            return _transforms.Where(t => t.layer.HasFlag(layer));
        }

        public IEnumerable<Transform> GetAllDirectedFromLayer(IntVector2 direction, Layer layer)
        {
            for (int i = _transforms.Count - 1; i >= 0; i--)
            {
                var t = _transforms[i];
                if (t.entity.IsDirected() && t.layer.HasFlag(layer) && t.orientation == direction)
                {
                    yield return t;
                }
            }
        }

        public Transform GetUndirectedTransformFromLayer(Layer layer)
        {
            return _transforms
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
            foreach (var t in _transforms)
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