using System.Collections.Generic;

namespace Core.Targeting
{
    public interface IProvideTargets<T, in M>
        where T : Target, new()
    {
        List<Target> GetTargets(TargetEvent<T> targetEvent, M meta);
        IEnumerable<T> GetParticularTargets(TargetEvent<T> targetEvent, M meta);
    }
}