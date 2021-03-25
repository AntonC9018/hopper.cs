using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public interface IWithChainTemplate
    {
        ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase;
    }
}