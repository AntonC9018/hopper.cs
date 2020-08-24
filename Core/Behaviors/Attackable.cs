using Chains;
using System.Collections.Generic;
using Core.FS;

namespace Core
{
    public enum AtkCondition
    {
        ALWAYS, NEVER, SKIP, IF_NEXT_TO
    }
    public class Attackable : Behavior
    {

        // this makes more sense on stats. Think about moving it there via some
        // e.g. extension api
        public static List<string> s_indexSourceNameMap = new List<string>();

        public static int RegisterAttackSource(string name, int defaultResValue = 1)
        {
            var sourceResFile = (ArrayFile)StatManager.s_defaultFS.GetFile("attacked/source_res");
            sourceResFile.content.Add(defaultResValue);

            s_indexSourceNameMap.Add(name);
            return s_indexSourceNameMap.Count - 1;
        }

        public static int BasicAttackSource = 0;

        static Attackable()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory attackDir = new Directory();
            File sourceResFile = new ArrayFile();
            File resFile = new Resistance
            {
                armor = 0,
                minDamage = 1,
                maxDamage = 10,
                pierce = 1
            };

            baseDir.AddDirectory("attacked", attackDir);
            attackDir.AddFile("source_res", sourceResFile);
            attackDir.AddFile("res", resFile);

            RegisterAttackSource("default");
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
            public Attacking.Attack attack;
            public Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attacking.Attack attack;
        }

        IChain chain_checkAttacked;
        IChain chain_beAttacked;
        IChain chain_getAttackableness;
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

        static void SetResistance(Event ev)
        {
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("attacked/res");
        }

        static void ResistSource(Event ev)
        {
            System.Console.WriteLine(ev.attack.source);
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("attacked/source_res");
            if (sourceRes[ev.attack.source] > ev.attack.power)
            {
                ev.attack.damage = 0;
            }
        }

        static void Armor(Event ev)
        {
            ev.attack.damage = System.Math.Clamp(
                ev.attack.damage - ev.resistance.armor,
                ev.resistance.minDamage,
                ev.resistance.maxDamage);

            if (ev.attack.pierce < ev.resistance.pierce)
            {
                ev.attack.damage = 0;
            }
        }

        static void TakeHit(Event ev)
        {
            System.Console.WriteLine($"Taken {ev.attack.damage} damage");
        }


        public class AttackablenessEvent : CommonEvent
        {
            public AtkCondition attackableness = AtkCondition.ALWAYS;
        }

        // TODO: this should get passed the attacker and the info about the attack
        // so Attacking.Event event        
        public AtkCondition GetAttackableness()
        {
            var ev = new AttackablenessEvent { actor = this.m_entity };
            chain_getAttackableness.Pass(ev);
            return ev.attackableness;
        }

        // I do hate the amount of boilerplate here
        // Since we want to have just one copy of this factory per class
        // I don't want to bloat my instances with copies of this
        public static BehaviorFactory<Attackable> s_factory = new BehaviorFactory<Attackable>(
            new IChainDef[]
            {
                new ChainDef<Event>
                {
                    name = "attacked:check",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event>(
                            SetResistance,
                            PRIORITY_RANKS.HIGH
                        ),
                        new EvHandler<Event>(
                            ResistSource,
                            PRIORITY_RANKS.LOW
                        ),
                        new EvHandler<Event>(
                            Armor,
                            PRIORITY_RANKS.LOW
                        )
                    }
                },
                new ChainDef<Event>
                {
                    name = "attacked:do",
                    handlers = new EvHandler<Event>[]
                    {
                        new EvHandler<Event>(
                            TakeHit
                        )
                    }
                },
                new ChainDef<AttackablenessEvent>
                {
                    name = "attacked:condition",
                    handlers = new EvHandler<AttackablenessEvent>[]
                    {
                    }
                }
            }
        );
    }
}