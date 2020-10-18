using Utils.MyLinkedList;

namespace Chains
{
    public class Handle { }

    public class Handle<Event> : Handle where Event : EventBase
    {
        public MyListNode<IEvHandler<Event>> Item { get; private set; }
        public Handle(MyListNode<IEvHandler<Event>> item)
        {
            Item = item;
        }
    }
}