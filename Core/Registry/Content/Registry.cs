using System;
using System.Collections.Generic;
using Hopper.Core.Mods;

namespace Hopper.Core.Registries
{
    public class Registry
    {
        private Dictionary<Type, IKindRegistry<IKind>> m_kindRegistries;
        private Dictionary<int, ModRegistry> m_modRegistries;

        public Registry()
        {
            m_kindRegistries = new Dictionary<Type, IKindRegistry<IKind>>();
            m_modRegistries = new Dictionary<int, ModRegistry>();
        }

        public ModRegistry CreateModRegistry(IMod mod)
        {
            var modRegistry = new ModRegistry(mod.Offset, m_kindRegistries);
            m_modRegistries.Add(mod.Offset, modRegistry);
            return modRegistry;
        }

        public IKindRegistry<T> GetKindRegistry<T>() where T : IKind
        {
            return (IKindRegistry<T>)m_kindRegistries[typeof(T)];
        }
    }
}