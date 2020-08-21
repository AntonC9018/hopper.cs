using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Displaceable : Behavior
    {
        static Displaceable()
        {
            var baseDir = StatManager.s_defaultStatsDir;

            var moveDir = new Directory<int>();
            moveDir.files = new Dictionary<string, int>
            {
                { "power", 1 },
                { "through", 0 }
            };

            baseDir.directories.Add("move", moveDir);
        }

        public class Move
        {
            public int power = 1;
            public int through = 0;

            public Move Copy()
            {
                return (Move)this.MemberwiseClone();
            }

            public static implicit operator Move(Dictionary<string, int> operand)
            {
                return new Move
                {
                    power = operand["power"],
                    through = operand["through"]
                };
            }
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

        Chain chain_checkDisplaced;
        Chain chain_beDisplaced;

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

        static void ConvertFromMove(EventBase eventBase)
        {
            var ev = (Event)eventBase;
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

        static void Displace(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            ev.actor.RemoveFromGrid();
            ev.actor.m_pos = ev.newPos;
            ev.actor.ResetInGrid();
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Displaceable), new ChainDefinition[]
            {
                new ChainDefinition
                {
                    name = "displaced:check",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = ConvertFromMove,
                            priority = (int)PRIORITY_RANKS.HIGH
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "displaced:do",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = Displace
                        }
                    }
                }
            }
        );
    }
}