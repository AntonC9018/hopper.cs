using System.Collections.Generic;

namespace Hopper.Core
{
    public struct StaticRegistry<T>
    {
        public IdentifierAssigner assigner;
        public Dictionary<Identifier, T> map;

        public void Init()
        {
            assigner = new IdentifierAssigner();
            map = new Dictionary<Identifier, T>();
        }        

        public Identifier Add(int modId, T thing)
        {
            var id = new Identifier(modId, assigner.Next());
            map.Add(id, thing);
            return id;
        }

        public void Remove(Identifier id)
        {
            map.Remove(id);
        }
    }
}