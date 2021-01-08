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
        private int[] m_priorityRanksMap = {
            PriorityMapping.Lowest,
            PriorityMapping.Low,
            PriorityMapping.Medium,
            PriorityMapping.High,
            PriorityMapping.Highest,
        };
        protected bool b_dirty;

        private MyLinkedList<IEvHandler<Event>> m_handlers;
        private List<MyListNode<IEvHandler<Event>>> m_handlersToRemove
            = new List<MyListNode<IEvHandler<Event>>>();

        public Dictionary<System.Action<Event>, Handle<Event>> priorities =
            new Dictionary<Action<Event>, Handle<Event>>();

        public IEnumerable<IEvHandler<Event>> Handlers =>
            m_handlers.GetEnumerator().ToIEnumerable();

        public Chain()
        {
            m_handlers = new MyLinkedList<IEvHandler<Event>>();
        }

        // Assumes the given list is sorted        
        public Chain(MyLinkedList<IEvHandler<Event>> list)
        {
            m_handlers = list;
            b_dirty = false;
        }

        public Handle<Event> AddHandler(IEvHandler<Event> handler)
        {
            b_dirty = true;
            var evHandlerCopy = new EvHandler<Event>(handler);
            evHandlerCopy.Priority = MapPriority(evHandlerCopy.Priority);
            m_handlers.AddFront(evHandlerCopy);
            return new Handle<Event>(m_handlers.Head);
        }

        public Handle<Event> AddHandler(
            System.Action<Event> handlerFunction,
            PriorityRank priority = PriorityRank.Default)
        {
            return AddHandler(new EvHandler<Event>(handlerFunction, priority));
        }

        public void Pass(Event ev)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (!ev.propagate)
                    return;
                handler.Call(ev);
            }
        }

        public void Pass(Event ev, System.Func<Event, bool> stopFunc)
        {
            CleanUp();
            foreach (var handler in m_handlers)
            {
                if (stopFunc(ev))
                    return;
                handler.Call(ev);
            }
        }

        public void RemoveHandler(MyListNode<IEvHandler<Event>> handle)
        {
            m_handlersToRemove.Add(handle);
        }

        public void RemoveHandler(Handle<Event> handle)
        {
            m_handlersToRemove.Add(handle.item);
        }

        public void RemoveHandler(Handle handle)
        {
            m_handlersToRemove.Add(((Handle<Event>)handle).item);
        }

        protected int MapPriority(int rank)
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

        internal void SortHandlers()
        {
            m_handlers.Sort((a, b) => a.Priority - b.Priority);
            b_dirty = false;
        }
    }
}