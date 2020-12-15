using System.Collections.Generic;

namespace Hopper.Core.Registry
{
    public class ModSubRegistry
    {
        private int m_offset;
        private Dictionary<System.Type, IKindRegistry<IKind>> m_kindSubRegistries;

        public ModSubRegistry(int offset, Dictionary<System.Type, IKindRegistry<IKind>> kindSubRegistries)
        {
            m_offset = offset;
            m_kindSubRegistries = kindSubRegistries;
        }

        public int Add<T>(T item) where T : IKind
        {
            if (!m_kindSubRegistries.ContainsKey(typeof(T)))
            {
                System.Console.WriteLine($"Not found a KindSubRegistry for the type {typeof(T)}. Creating one.");
                m_kindSubRegistries.Add(typeof(T), (IKindRegistry<IKind>)new KindSubRegistry<T>());
            }
            var registry = (KindSubRegistry<T>)m_kindSubRegistries[typeof(T)];
            return registry.Add(item, m_offset);
        }
    }
}