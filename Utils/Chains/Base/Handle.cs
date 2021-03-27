using System;
using Hopper.Utils.MyLinkedList;

namespace Hopper.Utils.Chains
{
    public class Handle { }

    public class Handle<Context> : Handle where Context : ContextBase
    {
        public MyListNode<Handler<Context>> item;
        public Handle(MyListNode<Handler<Context>> item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return item.ToString();
        }
    }
}