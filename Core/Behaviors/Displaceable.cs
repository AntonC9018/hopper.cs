using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core
{
    public class Displaceable : Behavior
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

        public class Move : File
        {
            public int power = 1;
            public int through = 0;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Move move;
            public Vector2 newPos;
        }

        public class Params : ActivationParams
        {
            public Move move;
        }

        Chain<CommonEvent> chain_checkDisplaced;
        Chain<CommonEvent> chain_beDisplaced;

        public Displaceable(Entity entity, BehaviorConfig conf)
        {
            chain_checkDisplaced = entity.m_chains["displaced:check"];
            chain_beDisplaced = entity.m_chains["displaced:do"];
        }

        public override bool Activate(
            Entity actor,
            Action action,
            ActivationParams pars = null)
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

        static void ConvertFromMove(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            int i = 1;
            for (; i < ev.move.power; i++)
            {
                var cell = ev.actor.GetCellRelative(ev.action.direction * i);

                if (cell == null || cell.GetEntityFromLayer(Layer.BLOCK) != null)
                {
                    i--;
                    break;
                }
            }
            ev.newPos = ev.actor.GetRelativePos(ev.action.direction * i);
        }

        static void Displace(CommonEvent commonEvent)
        {
            var ev = (Event)commonEvent;
            ev.actor.RemoveFromGrid();
            ev.actor.m_pos = ev.newPos;
            ev.actor.ResetInGrid();
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Displaceable), new ChainDef<CommonEvent>[]
            {
                new ChainDef<CommonEvent>
                {
                    name = "displaced:check",
                    handlers = new EvHandler<CommonEvent>[]
                    {
                        new EvHandler<CommonEvent> {
                            handlerFunction = ConvertFromMove,
                            priority = (int)PRIORITY_RANKS.HIGH
                        }
                    }
                },
                new ChainDef<CommonEvent>
                {
                    name = "displaced:do",
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