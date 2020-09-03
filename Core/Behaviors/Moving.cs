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
        public static string s_checkChainName = "move:check";
        public static string s_doChainName = "move:do";

        public Moving(Entity entity)
        {
        }

        public bool Activate(Entity actor, Action action)
        {
            var ev = new Event
            {
                actor = actor,
                action = action
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);

        }

        static void SetBase(Event ev)
        {
            if (ev.move == null)
            {
                // TODO: set stats for move
                ev.move = (Displaceable.Move)ev.actor.m_statManager.GetFile("move");
            }
        }

        static void Displace(Event ev)
        {
            var pars = new Displaceable.Params { move = ev.move };
            ev.actor.GetBehavior<Displaceable>().Activate(ev.actor, ev.action, pars);
        }

        public static ChainPath<Moving, Event> check_chain;
        public static ChainPath<Moving, Event> do_chain;

        static Moving()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            check_chain = new ChainPath<Moving, Event>(s_checkChainName);
            check.AddHandler(SetBase, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            do_chain = new ChainPath<Moving, Event>(s_doChainName);
            _do.AddHandler(Displace);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.move_do));

            BehaviorFactory<Moving>.s_builder = builder;
        }
    }
}