using System.Collections.Generic;

namespace Core.Targeting
{
    public interface IProvideTargets<out T, in M>
        where T : Target, new()
    {
        List<Target> GetTargets(M meta);
        IEnumerable<T> GetParticularTargets(M meta);
    }
}