using System.Collections.Generic;

namespace Core.Targeting
{
    public interface IProvideTargets<out T> where T : Target, new()
    {
        List<Target> GetTargets(CommonEvent commonEvent);
        IEnumerable<T> GetParticularTargets(CommonEvent commonEvent);
    }
}