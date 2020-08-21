using Chains;
using System.Collections.Generic;
namespace Core
{
    public class Attackable : Behavior
    {

        // this makes more sense on stats. Think about moving it there via some
        // e.g. extension api
        public static List<string> s_indexSourceNameMap = new List<string>();
        public static readonly string s_attackSourcePrefix = "as/";
        public static readonly string s_attackSourceResistancePrefix = "asr/";
        public static readonly string s_attackPrefix = "a/";

        public static int RegisterAttackSource(string name, int defaultValue)
        {
            StatManager.RegisterStat(s_attackSourcePrefix + name, defaultValue);
            StatManager.RegisterStatInCategory("AttackSource", name);
            StatManager.RegisterStat(s_attackSourceResistancePrefix + name, defaultValue);
            StatManager.RegisterStatInCategory("AttackSourceRes", name);
            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        public static int BasicAttackSource = 0;

        static Attackable()
        {
            StatManager.RegisterCategory("AttackSource");
            StatManager.RegisterCategory("AttackSourceRes");
            // Add the base type of attack, which is the default and its index is 0
            RegisterAttackSource("basic", 1);

            var baseAttack = new Attack();
            var attackStatNames = new List<string>
            {
                $"{s_attackPrefix}source",
                $"{s_attackPrefix}power",
                $"{s_attackPrefix}damage",
                $"{s_attackPrefix}pierce"
            };
            // StatManager.RegisterCategory(
        }

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
                return (Attack)this.MemberwiseClone();
            }
        }

        public class Resistance
        {
            public int armor = 0;
            public int minDamage = 1;
            public int maxDamage = 10;
            public int pierce = 1;

            public Resistance Copy()
            {
                return (Resistance)this.MemberwiseClone();
            }
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

        static void SetResistance(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            // TODO:
            ev.resistance = new Resistance();
        }

        static void ResistSource(EventBase eventBase)
        {
            // TODO
        }

        static void Armor(EventBase eventBase)
        {
            var ev = (Event)eventBase;

            ev.attack.damage = System.Math.Clamp(
                ev.attack.damage - ev.resistance.armor,
                ev.resistance.minDamage,
                ev.resistance.maxDamage);

            if (ev.attack.pierce < ev.resistance.pierce)
            {
                ev.attack.damage = 0;
            }
        }

        static void TakeHit(EventBase eventBase)
        {
            var ev = (Event)eventBase;
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