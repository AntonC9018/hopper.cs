using System.Collections.Generic;

namespace Hopper.Utils.Chains
{
    public class NonPriorityChain<EventData> : Chain
    {
        private LinkedList<System.Action<EventData>> m_handlers;
        private List<LinkedListNode<System.Action<EventData>>> m_handlersToRemove
            = new List<LinkedListNode<System.Action<EventData>>>();

        public NonPriorityChain()
        {
            m_handlers = new LinkedList<System.Action<EventData>>();
        }

        // Assumes the given list is sorted        
        public NonPriorityChain(LinkedList<System.Action<EventData>> list)
        {
            m_handlers = list;
        }

        public LinkedListNode<System.Action<EventData>> AddHandler(System.Action<EventData> handler)
        {
            return m_handlers.AddLast(handler);
        }

        public void Pass(EventData ev)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                handler(ev);
            }
        }

        public void RemoveHandler(LinkedListNode<System.Action<EventData>> handle)
        {
            m_handlersToRemove.Add(handle);
        }

        protected void CleanUp()
        {
            foreach (var handle in m_handlersToRemove)
            {
                m_handlers.Remove(handle);
            }
            m_handlersToRemove.Clear();
        }
    }
}