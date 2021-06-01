using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core
{
    public interface ISubRegistry<in T> 
    {
        Identifier Add(int modId, string name, T item);
    }

    public static class SubRegistryExtensions
    {
        public static Identifier AddForCurrentMod<T, V>(this ref T registry, string name, V item) where T : struct, ISubRegistry<V>
            => registry.Add(Registry.Global._currentMod, name, item);
    }


    public struct StaticGeneralRegistry<T, V> : ISubRegistry<V> where T : IDictionary<Identifier, V>
    {
        public IdentifierAssigner _assigner;
        public T _map; // I hate how there are going to be like 3 pointer dereferences to get to the thing here
                      // Like Global->Lookup(index)->map->thing
        public Dictionary<string, Identifier> _nameMap;

        public void Init(T map)
        {
            _map = map;
            _assigner = new IdentifierAssigner();
            _nameMap = new Dictionary<string, Identifier>();
        }

        public Identifier Add(int modId, string name, V item)
        {
            var id = new Identifier(modId, _assigner.Next());
            _map[id] = item;
            Assert.That(!_nameMap.ContainsKey(name), $"{name} has been added twice");
            _nameMap[name] = id;
            return id;
        }

        public void Remove(Identifier id) => _map.Remove(id);
        public V GetByName(string name) => (V) _map[_nameMap[name]];
    }

    public struct StaticRegistry<T> : ISubRegistry<T>
    {
        public IdentifierAssigner _assigner;
        public Dictionary<Identifier, T> _map;
        public Dictionary<string, Identifier> _nameMap;

        public void Init()
        {
            _assigner = new IdentifierAssigner();
            _map = new Dictionary<Identifier, T>();
            _nameMap = new Dictionary<string, Identifier>();
        }        

        public Identifier Add(int modId, string name, T thing)
        {
            var id = new Identifier(modId, _assigner.Next());
            _map.Add(id, thing);
            Assert.That(!_nameMap.ContainsKey(name), $"{name} has been added twice");
            _nameMap[name] = id;
            return id;
        }

        public void Remove(Identifier id) => _map.Remove(id);
        public T Get(Identifier identifier) => _map[identifier];
        public T GetByName(string name) => (T) _map[_nameMap[name]];
    }
}