using Hopper.Utils.Chains;

namespace Hopper.Core.Components
{
    public interface IWithChain
    {
        Chain<Event> GetChain<Event>(ChainName name) where Event : EventBase;
    }
}