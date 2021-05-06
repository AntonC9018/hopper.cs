using System.Collections.Generic;
using System.Linq;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface IGeneralizedPattern
    {
        IEnumerable<PositionAndDirection> GetPositionsAndDirections(IntVector2 position, IntVector2 direction);
    }
}