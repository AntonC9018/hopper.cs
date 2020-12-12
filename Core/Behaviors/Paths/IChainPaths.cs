using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public interface IChainPaths<Event> where Event : EventBase
    {
        Chain<Event> ChainPath(IProvideBehavior startingFrom);
        ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom);
    }
}