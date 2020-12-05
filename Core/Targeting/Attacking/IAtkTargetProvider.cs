using System.Collections.Generic;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    // Returns a standart Target enumerable, without any additional attack metadata like the atkness
    public interface IAtkTargetProvider
    {
        IEnumerable<Target> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack);
    }

    // AtkTarget has additional metadata about the attack. It includes the whole attack piece,
    // which has info on which index in the pattern resulted in a target and which positon was affected.
    // It also has info on atkness.
    // This might be needed for figuring out which swipe animation to play.
    // This is basically only useful for player weapons.
    public interface IBufferedAtkTargetProvider
    {
        IEnumerable<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 dir, Attack attack);
    }
}