using Chains;
using System.Collections.Generic;
namespace Core
{
    public class Attackable : Behavior
    {
        public Chain chain_beAttacked;
        public Chain chain_getAttackableness;
        public Entity m_entity;

        public Attackable(Entity entity)
        {
            chain_beAttacked = entity.m_chains["attacked:do"];
            chain_getAttackableness = entity.m_chains["attacked:condition"];
        }

        public override bool Activate(Entity actor, Action action)
        {
            var ev = new EventBase();
            chain_beAttacked.Pass(ev);
            return ev.propagate;
        }

        public enum Attackableness
        {
            ATTACKABLE, UNATTACKABLE, SKIP
        }

        public class AttackablenessEvent : EventBase
        {
            public Entity entity;
            public Attackableness attackableness = Attackableness.ATTACKABLE;
        }

        public Attackableness GetAttackableness()
        {
            var ev = new AttackablenessEvent { entity = this.m_entity };
            chain_getAttackableness.Pass(ev);
            return ev.attackableness;
        }

        static private void TestBeAttacked(EventBase e)
        {
            System.Console.WriteLine("Hello beAttacked");
        }
        static private void TestGetAttackableness(EventBase e)
        {
            System.Console.WriteLine("Hello beAttacked");
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Attackable), new ChainDefinition[]
            {
                new ChainDefinition
                {
                    name = "attacked:check",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            m_handlerFunction = TestBeAttacked
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "attacked:do",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            m_handlerFunction = TestBeAttacked
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "attacked:condition",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            m_handlerFunction = TestGetAttackableness
                        }
                    }
                }
            }
        );
    }
}