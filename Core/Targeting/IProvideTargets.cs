using System.Collections.Generic;

namespace Core.Targeting
{
    public interface IProvideTargets<out T, in E>
        where T : Target, new()
        where E : TargetEvent<T>
    {
        List<Target> GetTargets(E commonEvent);
        IEnumerable<T> GetParticularTargets(E commonEvent);
    }
}