using Hopper.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public class ChainPaths<Beh, Event> : IChainPaths<Event>
        where Beh : Behavior, new()
        where Event : EventBase
    {
        public readonly ChainName name;
        public ChainPaths(ChainName name)
        {
            this.name = name;
        }

        public Chain<Event> ChainPath(IProvideBehavior startingFrom)
        {
            return startingFrom.TryGet<Beh>()?.GetChain<Event>(name);
        }

        public ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom)
        {
            Assert.That(
                startingFrom.HasBehaviorFactory<Beh>(),
                $"Tried to add chains to a template of {typeof(Beh).Name}, but no such Behavior was present on the entity. Please, add the required behaviors prior to retouching"
            );
            return startingFrom.GetBehaviorFactory<Beh>().GetTemplate<Event>(name);
        }
    }
}