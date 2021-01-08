using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Predictions;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public interface IAnonPredictable
    {
        IEnumerable<IntVector2> Predict(IWorldSpot spot, IntVector2 direction);
    }
    public interface IAnonShooting : IAnonPredictable
    {
        ShootingInfo ShootAnon(IWorldSpot spot, IntVector2 direction);
    }
    public interface INormalShooting : IAnonPredictable
    {
        ShootingInfo Shoot(Entity entity, IntVector2 direction);
    }
}