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
        IEnumerable<IntVector2> Predict(Acting acting, IntVector2 direction);
    }
}