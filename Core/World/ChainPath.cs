using Chains;
using Core.Behaviors;

namespace Core
{
    public interface IProvideBehavior
    {
        public IProvidesChain GetBehavior<T>() where T : Behavior;
    }
    public interface IProvidesChain
    {
        public ICanAddHandlers<Event> GetChainLike<Event>(string name) where Event : EventBase;
    }
    public interface I_defines_path_to_chain<Event>
        where Event : EventBase
    {
        public ICanAddHandlers<Event> Path(IProvideBehavior entity);
    }
    public class ChainPath<Beh, Event>
        : I_defines_path_to_chain<Event>
        where Beh : Behavior
        where Event : EventBase
    {
        public string name;
        public ChainPath(string _name)
        {
            name = _name;
        }
        public ICanAddHandlers<Event> Path(IProvideBehavior entity)
        {
            return (ICanAddHandlers<Event>)entity.GetBehavior<Beh>().GetChainLike<Event>(name);
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