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

        public void Init()
        {
            _priority.Init();
            _entities.Init();
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

        public int NextPriority(PriorityRank rank)
        {
            return _priority.Next(rank);
        }
    }
}