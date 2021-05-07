using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    public struct Registry
    {
        public static Registry Global;
        
        static Registry()
        {
            Global = new Registry();
            Global.Init();
        }

        public int _currentMod;
        public IdentifierAssigner _component;
        public PriorityAssigner _priority;
        public RuntimeRegistry<Entity> _entities;
        public StaticRegistry<EntityFactory> _entityFactory;
        public IdentifierAssigner _stats;
        public IdentifierAssigner _slots;
        public StatsBuilder _defaultStats;
        public Pools _pools;
        

        public void Init()
        {
            _priority.Init();
            _entities.Init();
            _entityFactory.Init();
            _defaultStats = new StatsBuilder();
            _pools.Init();
        }

        public int NextMod()
        {
            return ++_currentMod;
        }

        public Identifier NextComponentId()
        {
            return new Identifier(_currentMod, _component.Next());
        }

        public RuntimeIdentifier RegisterRuntimeEntity(Entity entity)
        {
            return _entities.Add(entity);
        }

        public void UnregisterRuntimeEntity(Entity entity)
        {
            _entities.Remove(entity.id);
        }

        public Identifier RegisterEntityFactory(EntityFactory factory)
        {
            return _entityFactory.Add(_currentMod, factory);
        }

        public void UnregisterEntityFactory(EntityFactory factory)
        {
            _entityFactory.Remove(factory.id);
        }

        public Identifier RegisterStat(IStat stat)
        {
            var id = new Identifier(_currentMod, _stats.Next());
            _defaultStats[id] = stat;
            return id;
        }

        public Identifier NextSlotId()
        {
            return new Identifier(_currentMod, _slots.Next());
        }

        public int NextPriority(PriorityRank rank)
        {
            return _priority.Next(rank);
        }

        public void RegisterEntitySubPool(Identifier subpool)
        {

        }
    }
}