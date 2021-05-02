using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Predictions
{
    // public interface IPredictable
    // {
    // }
    public interface IBehaviorPredictable
    {
        IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info);
    }
}