using System.Collections.Generic;
using Hopper.Core.Predictions;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface IGeneralizedPattern
    {
        IEnumerable<TargetContext> MakeContexts(IntVector2 position, IntVector2 direction);
    }
}