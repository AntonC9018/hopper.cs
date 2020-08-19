using Chains;
using System.Collections.Generic;
namespace Core
{
    public class Attackable : Behavior
    {
        public enum Attackableness
        {
            ATTACKABLE, UNATTACKABLE, SKIP
        }

        public class Attack
        {
            public int source = 0;
            public int power = 1;
            public int damage = 1;
            public int pierce = 1;

            public Attack Copy()
            {
                // TODO
                return new Attack();
            }
        }

        public class Resistance
        {
            public int armor = 0;
            public int minDamage = 0;
            public int maxDamage = 1;
            public int pierce = 1;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Attack attack;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attack attack;
        }

        Chain chain_checkAttacked;
        Chain chain_beAttacked;
        Chain chain_getAttackableness;
        Entity m_entity;

        public Attackable(Entity entity, BehaviorConfig conf)
        {
            chain_checkAttacked = entity.m_chains["attacked:check"];
            chain_beAttacked = entity.m_chains["attacked:do"];
            chain_getAttackableness = entity.m_chains["attacked:condition"];
            m_entity = entity;
        }

        public override bool Activate(
            Entity actor,
            Action action,
            ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                attack = ((Params)pars).attack
            };
            chain_checkAttacked.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_beAttacked.Pass(ev);
            return true;
        }

        static void SetResistance(EventBase e)
        {
            var ev = (Event)e;
            // TODO:
            ev.resistance = new Resistance();
        }

        static void ResistSource(EventBase e)
        {
            // TODO
        }

        static void Armor(EventBase e)
        {
            var ev = (Event)e;

            ev.attack.damage = System.Math.Clamp(
                ev.attack.damage - ev.resistance.armor,
                ev.resistance.minDamage,
                ev.resistance.maxDamage);

            if (ev.attack.pierce <= ev.resistance.pierce)
            {
                ev.attack.damage = 0;
            }
        }

        static void TakeHit(EventBase e)
        {
            var ev = (Event)e;
            System.Console.WriteLine($"Taken {ev.attack.damage} damage");
        }


        public class AttackablenessEvent : EventBase
        {
            public Entity actor;
            public Attackableness attackableness = Attackableness.ATTACKABLE;
        }

        public Attackableness GetAttackableness()
        {
            var ev = new AttackablenessEvent { actor = this.m_entity };
            chain_getAttackableness.Pass(ev);
            return ev.attackableness;
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
                            handlerFunction = SetResistance,
                            priority = (int)PRIORITY_RANKS.HIGH
                        },
                        new WeightedEventHandler {
                            handlerFunction = ResistSource,
                            priority = (int)PRIORITY_RANKS.LOW
                        },
                        new WeightedEventHandler {
                            handlerFunction = Armor,
                            priority = (int)PRIORITY_RANKS.LOW
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "attacked:do",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                            handlerFunction = TakeHit
                        }
                    }
                },
                new ChainDefinition
                {
                    name = "attacked:condition",
                    handlers = new WeightedEventHandler[]
                    {
                        new WeightedEventHandler {
                        }
                    }
                }
            }
        );
    }
}