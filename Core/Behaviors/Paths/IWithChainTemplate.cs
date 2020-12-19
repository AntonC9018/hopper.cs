using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public interface IWithChainTemplate
    {
        ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase;
    }
}