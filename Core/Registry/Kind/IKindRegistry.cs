using System.Collections.Generic;

namespace Hopper.Core.Registries
{
    /*
        It assignes Id's to a class of objects. E.g. all `Items` will be assigned id's by
        and inserted into the Kind Registry
    */
    public interface IKindRegistry<out T>
    {
        IEnumerable<T> Items { get; }
        T Get(int id);
    }
}