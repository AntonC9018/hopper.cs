using Chains;
using System.Collections.Generic;
namespace Core
{
    public class Attackable : Behavior
    {

        // this makes more sense on stats. Think about moving it there via some
        // e.g. extension api
        public static List<string> s_indexSourceNameMap = new List<string>();

        public static int RegisterAttackSource(string name, int defaultResValue = 1)
        {
            var attackDir = StatManager.s_defaultStatsDir.directories["attack"];
            var sourceResDir = attackDir.directories["source_res"];
            sourceResDir.files.Add(name, defaultResValue);
            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        public static int BasicAttackSource = 0;

        static Attackable()
        {
            var baseDir = StatManager.s_defaultStatsDir;

            var attackDir = new Directory<int>();
            attackDir.files = new Dictionary<string, int>
            {
                { "source", 0 },
                { "power", 1 },
                { "damage", 1 },
                { "pierce", 1 }
            };

            var sourceResDir = new Directory<int>();
            sourceResDir.files = new Dictionary<string, int>
            {
                { "basic", 1 }
            };

            var resDir = new Directory<int>();
            resDir.files = new Dictionary<string, int>
            {
                { "armor", 0 },
                { "minDamage", 1 },
                { "maxDamage", 10 },
                { "pierce", 1 }
            };

            baseDir.directories.Add("attack", attackDir);
            attackDir.directories.Add("source_res", sourceResDir);
            attackDir.directories.Add("res", resDir);

            RegisterAttackSource("default");
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

            public static implicit operator Attack(Dictionary<string, int> operand)
            {
                return new Attack
                {
                    source = operand["source"],
                    power = operand["power"],
                    damage = operand["damage"],
                    pierce = operand["pierce"]
                };
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

            public static implicit operator Resistance(Dictionary<string, int> operand)
            {
                return new Resistance
                {
                    armor = operand["armor"],
                    minDamage = operand["minDamage"],
                    maxDamage = operand["maxDamage"],
                    pierce = operand["pierce"]
                };
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
            ev.resistance = ev.actor.m_statManager.GetStats("attack/res");
        }

        static void ResistSource(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            System.Console.WriteLine(ev.attack.source);
            var sourceName = s_indexSourceNameMap[ev.attack.source];
            var sourceRes = ev.actor.m_statManager.GetStats("attack/source_res");
            if (sourceRes[sourceName] > ev.attack.power)
            {
                ev.attack.damage = 0;
            }
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