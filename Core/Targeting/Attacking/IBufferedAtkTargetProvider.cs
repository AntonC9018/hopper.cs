using System.Collections.Generic;
using Hopper.Core.Predictions;
using Hopper.Core.Stat.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    // AtkTarget has additional metadata about the attack. It includes the whole attack piece,
    // which has info on which index in the pattern resulted in a target and which positon was affected.
    // It also has info on atkness.
    // This might be needed for figuring out which swipe animation to play.
    // This is basically only useful for player weapons.
    public interface IBufferedAtkTargetProvider : IWithPattern
    {
        List<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 direction);
    }
}