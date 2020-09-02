using System.Collections.Generic;
using Core.FS;
using Chains;

namespace Core.Behaviors
{
    public class Moving : IBehavior
    {

        public class Event : CommonEvent
        {
            public Displaceable.Move move;
        }
        public static string s_checkChainName = "move:check";
        public static string s_doChainName = "move:do";
        Chain<Event> chain_checkMove;
        Chain<Event> chain_doMove;

        public Moving(Entity entity)
        {
            chain_checkMove = (Chain<Event>)entity.m_chains[s_checkChainName];
            chain_doMove = (Chain<Event>)entity.m_chains[s_doChainName];
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                actor = actor,
                action = action
            };
            chain_checkMove.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_doMove.Pass(ev);

            return true;
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
            ev.actor.beh_Displaceable.Activate(ev.actor, ev.action, pars);
        }


        public static void SetupChainTemplates(BehaviorFactory<Moving> fact)
        {
            var check = fact.AddTemplate<Event>(s_checkChainName);
            var setBaseHandler = new EvHandler<Event>(SetBase, PRIORITY_RANKS.HIGH);
            check.AddHandler(setBaseHandler);

            var _do = fact.AddTemplate<Event>(s_doChainName);
            var displaceHandler = new EvHandler<Event>(Displace);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.move_do));
            _do.AddHandler(displaceHandler);
            _do.AddHandler(addEventHandler);
        }
        public static int id = BehaviorFactory<Moving>.ClassSetup(SetupChainTemplates);
    }
}