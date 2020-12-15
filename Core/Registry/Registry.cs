using System;
using System.Collections.Generic;
using Hopper.Core.Mods;

namespace Hopper.Core.Registry
{
    public class KindRegistry
    {
        private Dictionary<Type, IKindRegistry<IKind>> m_kindSubRegistries;
        private Dictionary<int, ModSubRegistry> m_modSubRegistries;

        public KindRegistry()
        {
            m_kindSubRegistries = new Dictionary<Type, IKindRegistry<IKind>>();
            m_modSubRegistries = new Dictionary<int, ModSubRegistry>();
        }

        public ModSubRegistry CreateModSubRegistry(IMod mod)
        {
            var subRegistry = new ModSubRegistry(mod.Offset, m_kindSubRegistries);
            m_modSubRegistries.Add(mod.Offset, subRegistry);
            return subRegistry;
        }
    }
}