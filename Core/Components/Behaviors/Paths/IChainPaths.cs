using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public interface IChainPath<Event> where Event : EventBase
    {
        Chain<Event> ChainPath(IWithWithChain startingFrom);
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