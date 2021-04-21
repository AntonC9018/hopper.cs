using System.Collections.Generic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class UnbufferedTargetProvider : IComponent
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
    }
}