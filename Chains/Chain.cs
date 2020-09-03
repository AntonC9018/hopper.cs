using System;
using System.Collections.Generic;
using MyLinkedList;

namespace Chains
{
    public interface ICanAddHandlers<Event> where Event : EventBase
    {
        public void AddHandler(EvHandler<Event> handler);
    }

    public enum PRIORITY_RANKS
    {
        HIGHEST = 4,
        HIGH = 3,
        MEDIUM = 2,
        LOW = 1,
        LOWEST = 0,
        DEFAULT = MEDIUM
    }
    public class EventBase
    {
        public bool propagate = true;
    }

    public abstract class IEvHandler
    {
        public int priority = (int)PRIORITY_RANKS.MEDIUM;

        public abstract void Call(EventBase ev);

        public IEvHandler Clone()
        {
            return (IEvHandler)this.MemberwiseClone();
        }
    }

    public class EvHandler<Event> : IEvHandler where Event : EventBase
    {
        System.Action<Event> handlerFunction;

        public EvHandler()
        {
        }

        public EvHandler(System.Action<Event> handlerFunc, PRIORITY_RANKS priority = PRIORITY_RANKS.DEFAULT)
        {
            handlerFunction = handlerFunc;
            this.priority = (int)priority;
        }

        public override void Call(EventBase ev)
        {
            handlerFunction((Event)ev);
        }
    }

    public abstract class Chain
    {
        const int NUM_PRIORITY_RANKS = (int)PRIORITY_RANKS.HIGHEST + 1;
        const int PRIORITY_STEP = 5;
        internal MyLinkedList<IEvHandler> m_handlers;
        protected List<MyListNode<IEvHandler>> m_handlersToRemove
            = new List<MyListNode<IEvHandler>>();

        private int[] m_priorityRanksMap = {
            5000, 6000, 7000, 8000, 9000
        };

        protected bool b_dirty;

        public MyListNode<IEvHandler> AddHandler<T>(
            System.Action<T> handlerFunction) where T : EventBase
        {
            return AddHandler(
                new EvHandler<T>(handlerFunction));
        }

        public MyListNode<IEvHandler> AddHandler(IEvHandler handler)
        {
            b_dirty = true;
            m_handlers.AddFront(handler);
            handler.priority = MapPriority(handler.priority);
            return m_handlers.Head;
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

        private void SortHandlers()
        {
            m_handlers.Sort((a, b) => a.priority - b.priority);
            b_dirty = false;
        }

        public void RemoveHandler(MyListNode<IEvHandler> handle)
        {
            m_handlersToRemove.Add(handle);
        }
    }

    public class Chain<Event> : Chain where Event : EventBase
    {
        public Chain()
        {
            m_handlers = new MyLinkedList<IEvHandler>();
        }

        // Assumes the given list is sorted        
        public Chain(MyLinkedList<IEvHandler> list)
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
    }
}