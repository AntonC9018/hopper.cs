using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
{
    public class Displaceable : IBehavior
    {
        static Displaceable()
        {
            var baseDir = StatManager.s_defaultFS.BaseDir;

            var move = new Move
            {
                power = 1,
                through = 0
            };

            baseDir.AddFile("move", move);
        }

        public class Move : StatFile
        {
            public int power = 1;
            public int through = 0;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
        }

        public class Params : ActivationParams
        {
            public Move move;
        }

        Chain<Event> chain_checkDisplaced;
        Chain<Event> chain_beDisplaced;

        public Displaceable(Entity entity)
        {
            chain_checkDisplaced = (Chain<Event>)entity.m_chains["displaced:check"];
            chain_beDisplaced = (Chain<Event>)entity.m_chains["displaced:do"];
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                move = ((Params)pars).move
            };
            chain_checkDisplaced.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_beDisplaced.Pass(ev);
            return true;
        }

        static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                var cell = ev.actor.GetCellRelative(ev.action.direction * i);

                if (cell == null || cell.GetEntityFromLayer(Layer.BLOCK) != null)
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetRelativePos(ev.action.direction * i);
        }

        static void Displace(Event ev)
        {
            ev.actor.RemoveFromGrid();
            ev.actor.m_pos = ev.newPos;
            ev.actor.ResetInGrid();
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory<Displaceable> s_factory = new BehaviorFactory<Displaceable>(
            new IChainDef[]
            {
                new ChainDef<Event>
                {
                    name = "displaced:check",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event>(
                            ConvertFromMove,
                            PRIORITY_RANKS.HIGH
                        )
                    }
                },
                new ChainDef<Event>
                {
                    name = "displaced:do",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event>(
                            Displace
                        ),
                        new EvHandler<Event>(
                            Utils.AddHistoryEvent(History.EventCode.displaced_do)
                        )
                    }
                }
            }
        );
    }
}