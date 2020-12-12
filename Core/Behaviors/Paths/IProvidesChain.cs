using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public interface IProvidesChain
    {
        Chain<Event> GetChain<Event>(ChainName name) where Event : EventBase;
    }
}