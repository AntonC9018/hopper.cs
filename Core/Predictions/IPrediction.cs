using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    public interface IPredictable
    {
    }

    public interface IDirectedPredictable : IPredictable
    {
        IEnumerable<IntVector2> GetPositions(IntVector2 direction);
    }

    public interface IUndirectedPredictable : IPredictable
    {
        IEnumerable<IntVector2> GetPositions();
    }
}