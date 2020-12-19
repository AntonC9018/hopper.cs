using Hopper.Utils.MyLinkedList;

namespace Hopper.Utils.Chains
{
    public class Handle { }

    public class Handle<Event> : Handle where Event : EventBase
    {
        public MyListNode<IEvHandler<Event>> item;
        public Handle(MyListNode<IEvHandler<Event>> item)
        {
            this.item = item;
        }
    }
}