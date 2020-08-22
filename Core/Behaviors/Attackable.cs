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
            var sourceResFile = (ArrayFile)StatManager.s_defaultFS.GetFile("attack/source_res");
            sourceResFile.content.Add(defaultResValue);

            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        public static int BasicAttackSource = 0;

        static Attackable()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory attackDir = new Directory();
            File baseFile = new Attack
            {
                source = 0,
                power = 1,
                damage = 1,
                pierce = 1
            };
            File sourceResFile = new ArrayFile();
            File resFile = new Resistance
            {
                armor = 0,
                minDamage = 1,
                maxDamage = 10,
                pierce = 1
            };

            baseDir.AddDirectory("attack", attackDir);
            attackDir.AddFile("base", baseFile);
            attackDir.AddFile("source_res", sourceResFile);
            attackDir.AddFile("res", resFile);

            RegisterAttackSource("default");
        }

        public enum Attackableness
        {
            ATTACKABLE, UNATTACKABLE, SKIP
        }

        public class Attack : File
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;
        }

        public class Resistance : File
        {
            public int armor;
            public int minDamage;
            public int maxDamage;
            public int pierce;
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
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("attack/res");
        }

        static void ResistSource(EventBase eventBase)
        {
            var ev = (Event)eventBase;
            System.Console.WriteLine(ev.attack.source);
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("attack/source_res");
            if (sourceRes[ev.attack.source] > ev.attack.power)
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