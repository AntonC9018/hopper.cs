using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public partial class UnbufferedTargetProvider : IComponent
    {
        [Inject] private IGeneralizedPattern _pattern;
        [Inject] private Layer _targetedLayer;

        public IEnumerable<TargetContext> GetTargets(IntVector2 position, IntVector2 direction)
        {
            foreach (var t in _pattern.MakeContexts(position, direction))
            {
                if (World.Global.grid.TryGetTransformFromLayer(
                    t.position, t.direction, _targetedLayer, out t.transform))
                {
                    yield return t;
                }
            }
        }

        public IEnumerable<TargetContext> GetTargetsDeep(IntVector2 position, IntVector2 direction)
        {
            foreach (var t in _pattern.MakeContexts(position, direction))
            {
                foreach (var transform in World.Global.grid.GetAllFromLayer(position, direction, _targetedLayer))
                {
                    var new_t = new TargetContext(transform);
                    yield return new_t;
                }
            }
        }
    }
}