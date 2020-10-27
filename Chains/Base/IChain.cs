using System;
using System.Collections.Generic;
using Core.Utils.MyLinkedList;

namespace Chains
{
    // public interface IChain<Event> where Event : EventBase
    // {
    //     IEnumerable<IEvHandler<Event>> Handlers { get; }
    //     MyListNode<IEvHandler<Event>> AddHandler(IEvHandler<Event> handler);
    //     MyListNode<IEvHandler<Event>> AddHandler(Action<Event> handlerFunction, PriorityRanks priority = PriorityRanks.Medium);
    //     void Pass(Event ev);
    //     void Pass(Event ev, Func<Event, bool> stopFunc);
    //     void RemoveHandler(MyListNode<IEvHandler<Event>> handle);
    //     void SortHandlers();
    // }
}