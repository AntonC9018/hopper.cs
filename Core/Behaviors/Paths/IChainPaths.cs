using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public interface IChainPath<Event> where Event : EventBase
    {
        Chain<Event> ChainPath(IProvideBehavior startingFrom);
    }

    public interface IChainTemplatePath<Event> where Event : EventBase
    {
        ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom);
    }

    public interface IChainPaths<Event> : IChainPath<Event>, IChainTemplatePath<Event>
        where Event : EventBase
    {
    }
}