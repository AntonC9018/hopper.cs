using System.Collections.Generic;
using Core.FS;
using Chains;

namespace Core.Behaviors
{
    public class Moving : Behavior, IStandartActivateable
    {

        public class Event : CommonEvent
        {
            public Displaceable.Move move;
        }

        public bool Activate(Action action)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetBase(Event ev)
        {
            if (ev.move == null)
            {
                // TODO: set stats for move
                ev.move = (Displaceable.Move)ev.actor.StatManager.GetFile("move");
            }
        }

        static void Displace(Event ev)
        {
            var pars = new Displaceable.Params(ev.move);
            ev.actor.GetBehavior<Displaceable>().Activate(ev.action, pars);
        }

        public static ChainPaths<Moving, Event> Check;
        public static ChainPaths<Moving, Event> Do;

        static Moving()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(ChainName.Check);
            Check = new ChainPaths<Moving, Event>(ChainName.Check);
            check.AddHandler(SetBase, PriorityRanks.High);

            var _do = builder.AddTemplate<Event>(ChainName.Do);
            Do = new ChainPaths<Moving, Event>(ChainName.Do);
            _do.AddHandler(Displace);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.move_do));

            BehaviorFactory<Moving>.s_builder = builder;
        }
    }
}