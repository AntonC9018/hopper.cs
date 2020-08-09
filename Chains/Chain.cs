using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public enum PRIORITY_RANKS
    {
        HIGHEST = 4,
        HIGH = 3,
        MEDIUM = 2,
        LOW = 1,
        LOWEST = 0
    }
    public class EventBase
    {
        public bool b_propagate = true;
    }

    public class WeightedEventHandler
    {
        public int m_priority = (int)PRIORITY_RANKS.MEDIUM;
        public System.Action<EventBase> m_handlerFunction;
    }

    public class Chain
    {
        const int NUM_PRIORITY_RANKS = (int)PRIORITY_RANKS.HIGHEST + 1;

        const int PRIORITY_STEP = 5;

        private int[] m_priorityRanksMap = {
            9000, 8000, 7000, 6000, 5000
        };

        // This should not be referenced by anything outside the namespace
        // The reason it's not private is because this has to be referenced 
        // by ChainTemplate, which is kind of logically tied to this class
        internal MyLinkedList<WeightedEventHandler> m_handlers { get; }

        private List<MyListNode<WeightedEventHandler>> m_handlersToRemove
            = new List<MyListNode<WeightedEventHandler>>();

        private bool b_dirty = true;


        public Chain()
        {
            m_handlers = new MyLinkedList<WeightedEventHandler>();
        }

        // Assumes the given list is sorted        

        public Chain(MyLinkedList<WeightedEventHandler> list)
        {
            m_handlers = list;
            b_dirty = false;
        }

        public void Pass(EventBase ev)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (!ev.b_propagate)
                    return;
                handler.m_handlerFunction(ev);

            }
        }

        public void Pass(EventBase ev, System.Func<EventBase, bool> stopFunc)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (stopFunc(ev))
                    return;
                handler.m_handlerFunction(ev);
            }
        }

        public MyListNode<WeightedEventHandler> AddHandler(
            WeightedEventHandler handler)
        {
            b_dirty = true;
            m_handlers.AddFront(handler);
            handler.m_priority = MapPriority(handler.m_priority);
            return m_handlers.Head;
        }

        public MyListNode<WeightedEventHandler> AddHandler(
            System.Action<EventBase> handlerFunction)
        {
            return AddHandler(
                new WeightedEventHandler
                {
                    m_handlerFunction = handlerFunction
                }
            );
        }

        public void RemoveHandler(MyListNode<WeightedEventHandler> handle)
        {
            m_handlersToRemove.Add(handle);
        }


        private int MapPriority(int rank)
        {
            // given a rank
            if (rank < NUM_PRIORITY_RANKS)
            {
                m_priorityRanksMap[rank] -= PRIORITY_STEP;

                return m_priorityRanksMap[rank];
            }

            // otherwise we are given a priority score
            return rank;
        }

        private void CleanUp()
        {
            foreach (var handle in m_handlersToRemove)
            {
                m_handlers.RemoveNode(handle);
            }
            if (b_dirty)
            {
                SortHandlers();
            }
            m_handlersToRemove.Clear();
        }

        private void SortHandlers()
        {
            m_handlers.Sort((a, b) => a.m_priority - b.m_priority);
            b_dirty = false;
        }
    }
}