using System;
using System.Collections.Generic;
using Hopper.Utils;
using Hopper.Utils.MyLinkedList;

namespace Hopper.Utils.Chains
{
    public class Chain { }

    public class Chain<Event> : Chain where Event : EventBase
    {
        public const int NUM_PRIORITY_RANKS = (int)PriorityRank.Highest + 1;
        public const int PRIORITY_STEP = 0x08;
        internal int[] m_priorityRanksMap = {
            PriorityMapping.Lowest,
            PriorityMapping.Low,
            PriorityMapping.Medium,
            PriorityMapping.High,
            PriorityMapping.Highest,
        };
        internal bool m_dirty;
        internal MyLinkedList<Action<Event>> m_handlers;
        internal List<MyListNode<Action<Event>>> m_handlersToRemove;
        internal Dictionary<MyListNode<Action<Event>>, int> m_priorities;

        public Chain()
        {
            m_handlers = new MyLinkedList<Action<Event>>();
            m_priorities = new Dictionary<MyListNode<Action<Event>>, int>();
            m_handlersToRemove = new List<MyListNode<Action<Event>>>();
        }

        public Handle<Event> AddHandler(Action<Event> handlerFunction,
            PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(handlerFunction, MapPriority((int)priority));
        }

        public Handle<Event> AddHandler(Action<Event> handlerFunction, int priority)
        {
            m_dirty = true;
            m_handlers.AddFront(handlerFunction);
            m_priorities.Add(m_handlers.head, priority);
            return new Handle<Event>(m_handlers.head);
        }

        public void Pass(Event ev)
        {
            CleanUp();
            foreach (var node in m_handlers)
            {
                if (!ev.propagate)
                    return;
                node.item(ev);
            }
        }

        public void Pass(Event ev, System.Func<Event, bool> stopFunc)
        {
            CleanUp();
            foreach (var node in m_handlers)
            {
                if (stopFunc(ev))
                    return;
                node.item(ev);
            }
        }

        public void RemoveHandler(Handle<Event> handle)
        {
            m_handlersToRemove.Add(handle.item);
        }

        public void RemoveHandler(Handle handle)
        {
            m_handlersToRemove.Add(((Handle<Event>)handle).item);
        }

        internal int MapPriority(int rank)
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

        protected void CleanUp()
        {
            foreach (var node in m_handlersToRemove)
            {
                m_handlers.RemoveNode(node);
                m_priorities.Remove(node);
            }
            if (m_dirty)
            {
                SortHandlers();
            }
            m_handlersToRemove.Clear();
        }

        internal void SortHandlers()
        {
            m_handlers.Sort((a, b) => m_priorities[a] - m_priorities[b]);
            m_dirty = false;
        }
    }
}