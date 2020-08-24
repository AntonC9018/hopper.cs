using System.Collections.Generic;
using Core.FS;
using Chains;

namespace Core
{
    public class Moving : Behavior
    {

        public class Event : CommonEvent
        {
            public Displaceable.Move move;
        }

        Chain<CommonEvent> chain_checkMove;
        Chain<CommonEvent> chain_doMove;

        public Moving(Entity entity, BehaviorConfig conf)
        {
            chain_checkMove = entity.m_chains["move:check"];
            chain_doMove = entity.m_chains["move:do"];
        }

        public override bool Activate(Entity actor, Action action, ActivationParams pars)
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

        static void SetBase(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            if (ev.move == null)
            {
                // TODO: set stats for move
                ev.move = (Displaceable.Move)ev.actor.m_statManager.GetFile("move");
            }
        }

        static void Displace(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            var pars = new Displaceable.Params { move = ev.move };
            ev.actor.beh_Displaceable.Activate(ev.actor, ev.action, pars);
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Moving), new ChainDef<CommonEvent>[]
            {
                new ChainDef<CommonEvent>
                {
                    name = "move:check",
                    handlers = new EvHandler<CommonEvent>[]
                    {
                        new EvHandler<CommonEvent> {
                            handlerFunction = SetBase,
                            priority = (int)PRIORITY_RANKS.HIGH
                        }
                    }
                },
                new ChainDef<CommonEvent>
                {
                    name = "move:do",
                    handlers = new EvHandler<CommonEvent>[]
                    {
                        new EvHandler<CommonEvent> {
                            handlerFunction = Displace
                        }
                    }
                }
            }
        );
    }
}