using Chains;
using Core.Behaviors;

namespace Core
{
    public interface IProvideBehavior
    {
        public T GetBehavior<T>() where T : Behavior;
    }
    public interface IProvideBehaviorFactory
    {
        public BehaviorFactory<T> GetBehaviorFactory<T>() where T : Behavior;
    }
    public interface IProvidesChain
    {
        public Chain<Event> GetChain<Event>(string name) where Event : EventBase;
    }
    public interface IProvidesChainTemplate
    {
        public ChainTemplate<Event> GetTemplate<Event>(string name) where Event : EventBase;
    }
    public class ChainPaths<Beh, Event>
        where Beh : Behavior
        where Event : EventBase
    {
        public string name;
        public ChainPaths(string _name)
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

    // public class ChainManager
    // {
    //     public List<
    //     public void AddChain<Event> where Event : EventBase
    //     {
    //         return this;
    //     }
    //     public ICanAddHandlers<Event> GetChain<Event>(int id) where Event : EventBase
    //     {
    //         throw new System.NotImplementedException();
    //     }
    // }
}