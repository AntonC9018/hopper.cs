using System;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Core
{
    /// <summary>
    /// A registry extension is another smaller registry, that is added to the global Registy
    /// at content initilization time by mods.
    /// </summary>
    public interface IRegistryExtension {}

    public struct RegistryExtensionPath<T> where T : IRegistryExtension
    {
        public int modId;
        public T Get() => Get(Registry.Global);
        public T Get(Registry registry) => (T) registry._extensions[modId];
    }

    public struct Registry
    {
        [RequiringInit] public static Registry Global;
        
        static Registry()
        {
            Global = new Registry();
            Global.BeforeModsInitialize();
        }

        public int _currentMod;
        public Dictionary<int, IRegistryExtension> _extensions;

        public void Extend(IRegistryExtension registryExt)
        {
            Assert.That(!_extensions.ContainsKey(_currentMod), "The given mod has already extended the registry");
            _extensions.Add(_currentMod, registryExt);
        }

        public PriorityAssigner Priority;
        public RuntimeRegistry<Entity> RuntimeEntities;
        public StaticRegistry<EntityFactory> EntityFactory;
        public IdentifierAssigner Component;
        
        public IdentifierAssigner Slot;
        public Pools Pools;

        public StaticGeneralRegistry<StatsBuilder, IStat> Stats; 
        public StaticGeneralRegistry<ChainsBuilder, IChain> MoreChains; 
        public StaticGeneralRegistry<ChainsBuilder, IChain> GlobalChains; 

        public StaticRegistry<ActingNS.InputMapping> InputMappings;
        public Queries Queries;
        
        public void BeforeModsInitialize()
        {
            Priority.Init();
            RuntimeEntities.Init();
            EntityFactory.Init();
            Pools.Init();
            Stats.Init(new StatsBuilder());
            MoreChains.Init(new ChainsBuilder());
            GlobalChains.Init(new ChainsBuilder());
            Component = new IdentifierAssigner();
            Slot = new IdentifierAssigner();
            InputMappings.Init();
            _extensions = new Dictionary<int, IRegistryExtension>();
        }

        /// <summary>
        /// Added this as a temporary solution.
        /// </summary>
        public void Init()
        {
            Queries.Init();
        }

        public int NextMod()
        {
            // TODO: reset identifier assigners to 0
            return ++_currentMod;
        }
    }
}