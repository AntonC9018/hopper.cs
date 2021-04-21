using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class StraightPattern : IGeneralizedPattern
    {
        private Layer _stopSearchLayer;

        public StraightPattern(Layer stopSearchLayer)
        {
            _stopSearchLayer = stopSearchLayer;
        }

        public IEnumerable<TargetContext> MakeContexts(IntVector2 position, IntVector2 direction)
        {
            position += direction;

            while (World.Global.grid.IsInBounds(position) 
                && World.Global.grid.HasNoTransformAt(position, direction, _stopSearchLayer))
            {
                yield return new TargetContext(position, direction);
                position += direction;
            }
        }
    }
}