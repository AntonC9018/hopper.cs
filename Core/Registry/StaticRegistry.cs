using System.Collections.Generic;

namespace Hopper.Core
{
    public interface ISubRegistry<in T> 
    {
        Identifier Add(int modId, T item);
    }

    public static class SubRegistryExtensions
    {
        public static Identifier AddForCurrentMod<T, V>(this ref T registry, V item) where T : struct, ISubRegistry<V>
            => registry.Add(Registry.Global._currentMod, item);
    }


    public struct StaticGeneralRegistry<T, V> : ISubRegistry<V> where T : IDictionary<Identifier, V>
    {
        public IdentifierAssigner _assigner;
        public T _map; // I hate how there are going to be like 3 pointer dereferences to get to the thing here
                      // Like Global->Lookup(index)->map->thing

        public void Init(T map)
        {
            _map = map;
            _assigner = new IdentifierAssigner();
        }

        public Identifier Add(int modId, V item)
        {
            var id = new Identifier(modId, _assigner.Next());
            _map[id] = item;
            return id;
        }

        public void Remove(Identifier id) => _map.Remove(id);
    }

    public struct StaticRegistry<T> : ISubRegistry<T>
    {
        public IdentifierAssigner _assigner;
        public Dictionary<Identifier, T> _map;

        public void Init()
        {
            _assigner = new IdentifierAssigner();
            _map = new Dictionary<Identifier, T>();
        }        

        public Identifier Add(int modId, T thing)
        {
            var id = new Identifier(modId, _assigner.Next());
            _map.Add(id, thing);
            return id;
        }

        public void Remove(Identifier id) => _map.Remove(id);
        public T Get(Identifier identifier) => _map[identifier];
    }
}