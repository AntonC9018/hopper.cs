using System.Collections.Generic;

namespace Hopper.Core
{
    public struct RuntimeRegistry<T>
    {
        public IdentifierAssigner assigner;
        public Dictionary<RuntimeIdentifier, T> map;

        public void Init()
        {
            assigner = new IdentifierAssigner();
            map = new Dictionary<RuntimeIdentifier, T>();
        }        

        public RuntimeIdentifier Add(T thing)
        {
            var id = new RuntimeIdentifier(assigner.Next());
            map.Add(id, thing);
            return id;
        }

        public void Remove(RuntimeIdentifier id)
        {
            map.Remove(id);
        }

        public void Clear()
        {
            map.Clear();
            assigner = new IdentifierAssigner();
        }
    }
}