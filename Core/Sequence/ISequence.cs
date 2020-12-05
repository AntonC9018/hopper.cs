using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public interface ISequence
    {
        Action CurrentAction { get; }
        List<IntVector2> GetMovs(Entity actor);
        void TickAction(Entity actor);
    }
}