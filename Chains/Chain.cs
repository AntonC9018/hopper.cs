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
        public bool propagate = true;
    }

    public class EvHandler<Event> where Event : EventBase
    {
        public int priority = (int)PRIORITY_RANKS.MEDIUM;
        public System.Action<Event> handlerFunction;

        public EvHandler()
        {
        }

        public EvHandler(System.Action<Event> handlerFunc)
        {
            handlerFunction = handlerFunc;
        }
    }

    public class Chain<Event> where Event : EventBase
    {
        const int NUM_PRIORITY_RANKS = (int)PRIORITY_RANKS.HIGHEST + 1;

        const int PRIORITY_STEP = 5;

        private int[] m_priorityRanksMap = {
            5000, 6000, 7000, 8000, 9000
        };

        // This should not be referenced by anything outside the namespace
        // The reason it's not private is because this has to be referenced 
        // by ChainTemplate, which is kind of logically tied to this class
        internal MyLinkedList<EvHandler<Event>> m_handlers { get; }

        private List<MyListNode<EvHandler<Event>>> m_handlersToRemove
            = new List<MyListNode<EvHandler<Event>>>();

        private bool b_dirty = true;


        public Chain()
        {
            m_handlers = new MyLinkedList<EvHandler<Event>>();
        }

        // Assumes the given list is sorted        

        public Chain(MyLinkedList<EvHandler<Event>> list)
        {
            m_handlers = list;
            b_dirty = false;
        }

        public void Pass(Event ev)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (!ev.propagate)
                    return;
                handler.handlerFunction(ev);

            }
        }

        public void Pass(Event ev, System.Func<Event, bool> stopFunc)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (stopFunc(ev))
                    return;
                handler.handlerFunction(ev);
            }
        }

        public MyListNode<EvHandler<Event>> AddHandler(
            EvHandler<Event> handler)
        {
            b_dirty = true;
            m_handlers.AddFront(handler);
            handler.priority = MapPriority(handler.priority);
            return m_handlers.Head;
        }

        public MyListNode<EvHandler<Event>> AddHandler(
            System.Action<Event> handlerFunction)
        {
            return AddHandler(
                new EvHandler<Event>
                {
                    handlerFunction = handlerFunction
                }
            );
        }

        public void RemoveHandler(MyListNode<EvHandler<Event>> handle)
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
            m_handlers.Sort((a, b) => a.priority - b.priority);
            b_dirty = false;
        }
    }
}