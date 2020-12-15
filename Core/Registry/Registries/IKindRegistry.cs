using System.Collections.Generic;

namespace Hopper.Core.Registry
{
    public interface IKindRegistry<out T>
    {
        IEnumerable<T> Items { get; }
        T Get(int id);
    }
}