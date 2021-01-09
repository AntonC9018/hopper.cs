using System;
using Hopper.Utils.MyLinkedList;

namespace Hopper.Utils.Chains
{
    public class Handle { }

    public class Handle<Event> : Handle where Event : EventBase
    {
        public MyListNode<Handler<Event>> item;
        public Handle(MyListNode<Handler<Event>> item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return item.ToString();
        }
    }
}