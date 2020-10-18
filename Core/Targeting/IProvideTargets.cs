using System.Collections.Generic;

namespace Core.Targeting
{
    public interface IProvideTargets
    {
        List<Target> GetTargets(CommonEvent commonEvent);
    }
}