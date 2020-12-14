using System;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Mods;

namespace Hopper.Core
{
    public class KindRegistry
    {
        private Dictionary<Type, IKindRegistry<IKind>> m_kindSubRegistries;
        private Dictionary<int, ModSubRegistry> m_modSubRegistries;

        public KindRegistry()
        {
            m_kindSubRegistries = new Dictionary<Type, IKindRegistry<IKind>>
            {
                { typeof(ITinker), new KindSubRegistry<ITinker>() },
                { typeof(Retoucher), new KindSubRegistry<Retoucher>()},
                { typeof(IFactory<Entity>), new KindSubRegistry<IFactory<Entity>>() },
                { typeof(IItem), new KindSubRegistry<IItem>() },
                { typeof(IWorldEvent), new KindSubRegistry<IWorldEvent>() },
                { typeof(IStatus), new KindSubRegistry<IStatus>() }
            };
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