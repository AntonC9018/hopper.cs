using System.Collections.Generic;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public interface ISequence
    {
        Action CurrentAction { get; }
        List<IntVector2> GetMovs(Entity actor);
        void ApplyCurrentAlgo(Acting.Event ev);
        void TickAction(Entity actor);
    }
}