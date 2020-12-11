using System.Collections.Generic;

namespace Hopper.Core
{
    public interface IKindRegistry<out T> where T : IKind
    {
        IEnumerable<T> Items { get; }
        T Get(int id);
    }
}