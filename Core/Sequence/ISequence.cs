using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core
{
    public interface ISequence
    {
        Action CurrentAction { get; }
        List<IntVector2> GetMovs(Entity actor);
        void TickAction(Entity actor);
    }
}