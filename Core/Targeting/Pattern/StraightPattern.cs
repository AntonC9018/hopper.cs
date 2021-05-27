using System.Collections.Generic;
using Hopper.Core.WorldNS;
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

        public IEnumerable<PositionAndDirection> GetPositionsAndDirections(IntVector2 position, IntVector2 direction)
        {
            position += direction;

            while (World.Global.Grid.IsInBounds(position) 
                && World.Global.Grid.HasNoTransformAt(position, direction, _stopSearchLayer))
            {
                yield return new PositionAndDirection(position, direction);
                position += direction;
            }
        }
    }
}