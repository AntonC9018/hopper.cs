using System.Diagnostics;
using Hopper.Core.Chains;
using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public interface IProvideBehavior
    {
        T Get<T>() where T : Behavior, new();
        T TryGet<T>() where T : Behavior, new();
        bool Has<T>() where T : Behavior, new();
    }
    public interface IProvideBehaviorFactory
    {
        BehaviorFactory<T> GetBehaviorFactory<T>() where T : Behavior, new();
        bool HasBehaviorFactory<T>() where T : Behavior, new();
    }
    public interface IProvidesChain
    {
        Chain<Event> GetChain<Event>(ChainName name) where Event : EventBase;
    }
    public interface IProvidesChainTemplate
    {
        ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase;
    }

    public interface IChainPaths<Event> where Event : EventBase
    {
        Chain<Event> ChainPath(IProvideBehavior startingFrom);
        ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom);
    }

    public class ChainPaths<Beh, Event> : IChainPaths<Event>
        where Beh : Behavior, new()
        where Event : EventBase
    {
        public ChainName name;
        public ChainPaths(ChainName _name)
        {
            name = _name;
        }
        public Chain<Event> ChainPath(IProvideBehavior startingFrom)
        {
            return startingFrom.TryGet<Beh>()?.GetChain<Event>(name);
        }

        public ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom)
        {
            Debug.Assert(
                startingFrom.HasBehaviorFactory<Beh>(),
                $"Tried to add chains to a template of {typeof(Beh).Name}, but no such Behavior was present on the entity. Please, add the required behaviors prior to retouching"
            );
            return startingFrom.GetBehaviorFactory<Beh>().GetTemplate<Event>(name);
        }
    }
}