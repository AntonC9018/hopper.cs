using Chains;
using Core.Behaviors;

namespace Core.Behaviors
{
    public interface IProvideBehavior
    {
        public T GetBehavior<T>() where T : Behavior, new();
    }
    public interface IProvideBehaviorFactory
    {
        public BehaviorFactory<T> GetBehaviorFactory<T>() where T : Behavior, new();
    }
    public interface IProvidesChain
    {
        public Chain<Event> GetChain<Event>(ChainName name) where Event : EventBase;
    }
    public interface IProvidesChainTemplate
    {
        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase;
    }
    public class ChainPaths<Beh, Event>
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
            return startingFrom.GetBehavior<Beh>().GetChain<Event>(name);
        }

        public ChainTemplate<Event> TemplatePath(IProvideBehaviorFactory startingFrom)
        {
            return startingFrom.GetBehaviorFactory<Beh>().GetTemplate<Event>(name);
        }
    }
}