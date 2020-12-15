namespace Hopper.Core.Registry
{
    public class InstanceRegistry
    {
        public InstanceSubRegistry<Entity, FactoryLink> Entity = new InstanceSubRegistry<Entity, FactoryLink>();
        public InstanceRegistry<World> World = new InstanceRegistry<World>();
    }
}