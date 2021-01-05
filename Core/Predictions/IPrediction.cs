using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    public interface INeutralPredictable
    {
        IEnumerable<IntVector2> GetNeutralPositions(IntVector2 direction);
    }
    public interface IGoodPredictable
    {
        IEnumerable<IntVector2> GetGoodPositions(IntVector2 direction);
    }

    public interface IBadPredictable
    {
        IEnumerable<IntVector2> GetBadPositions(IntVector2 direction);
    }
}