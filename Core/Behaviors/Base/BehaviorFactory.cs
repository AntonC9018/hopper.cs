using Hopper.Core.Chains;
using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public class ConfigurableBehaviorFactory<T, Config> : IBehaviorFactory<T>, IWithChainTemplate
           where T : Behavior, IInitable<Config>, new()
    {
        public Config config;
        public ConfiglessBehaviorFactory<T> configlessFactory;

        public ConfigurableBehaviorFactory(ChainTemplateBuilder builder, Config config)
        {
            this.config = config;
            configlessFactory = new ConfiglessBehaviorFactory<T>(builder);
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return configlessFactory.GetTemplate<Event>(name);
        }

        public T Instantiate(Entity entity)
        {
            var behavior = configlessFactory.Instantiate(entity);
            behavior.Init(config);
            return behavior;
        }
    }
}