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

        Chain<Event> chain_checkMove;
        Chain<Event> chain_doMove;

        public Moving(Entity entity)
        {
            chain_checkMove = (Chain<Event>)entity.m_chains["move:check"];
            chain_doMove = (Chain<Event>)entity.m_chains["move:do"];
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


        public static BehaviorFactory<Moving> CreateFactory()
        {
            var fact = new BehaviorFactory<Moving>();

            var check = fact.AddTemplate<Event>("move:check");
            var setBaseHandler = new EvHandler<Event>(SetBase, PRIORITY_RANKS.HIGH);
            check.AddHandler(setBaseHandler);

            var _do = fact.AddTemplate<Event>("move:do");
            var displaceHandler = new EvHandler<Event>(Displace);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.move_do));
            _do.AddHandler(displaceHandler);
            _do.AddHandler(addEventHandler);

            return fact;
        }
        public static BehaviorFactory<Moving> s_factory = CreateFactory();
    }
}