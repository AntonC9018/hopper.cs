using System.Collections.Generic;

namespace Hopper.Core.Registry
{
    public class ModRegistry
    {
        private int m_offset;
        private Dictionary<System.Type, IKindRegistry<IKind>> m_kindRegistries;

        public ModRegistry(int offset, Dictionary<System.Type, IKindRegistry<IKind>> kindRegistries)
        {
            m_offset = offset;
            m_kindRegistries = kindRegistries;
        }

        public int Add<T>(T item) where T : IKind
        {
            if (!m_kindRegistries.ContainsKey(typeof(T)))
            {
                System.Console.WriteLine($"Not found a KindSubRegistry for the type {typeof(T)}. Creating one.");
                m_kindRegistries.Add(typeof(T), (IKindRegistry<IKind>)new KindRegistry<T>());
            }
            var registry = (KindRegistry<T>)m_kindRegistries[typeof(T)];
            return registry.Add(item, m_offset);
        }
    }
}