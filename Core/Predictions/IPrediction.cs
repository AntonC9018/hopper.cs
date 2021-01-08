using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    // public interface IPredictable
    // {
    // }
    public interface IBehaviorPredictable
    {
        IEnumerable<IntVector2> Predict(IntVector2 direction);
    }
}