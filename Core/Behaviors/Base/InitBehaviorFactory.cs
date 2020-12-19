using Hopper.Core.Chains;
using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public class InitableBehaviorFactory<T> : IBehaviorFactory<T>
        where T : Behavior, IInitable, new()
    {
        public ConfiglessBehaviorFactory<T> configlessFactory;

        public InitableBehaviorFactory(ChainTemplateBuilder builder)
        {
            configlessFactory = new ConfiglessBehaviorFactory<T>(builder);
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return configlessFactory.GetTemplate<Event>(name);
        }

        public T Instantiate(Entity entity)
        {
            var behavior = configlessFactory.Instantiate(entity);
            behavior.Init();
            return behavior;
        }
    }
}