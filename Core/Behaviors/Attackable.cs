using Chains;
using System.Collections.Generic;
using Core.FS;

namespace Core.Behaviors
{
    public enum AtkCondition
    {
        ALWAYS,
        NEVER,
        SKIP,
        IF_NEXT_TO
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

        static void SetupStats()
        {
            Directory baseDir = StatManager.s_defaultFS.BaseDir;

            Directory attackDir = new Directory();
            StatFile sourceResFile = new ArrayFile();
            StatFile resFile = new Resistance
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

        public class Resistance : StatFile
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

        public static string s_checkChainName = "attacked:check";
        public static string s_doChainName = "attacked:do";
        public static string s_conditionChainName = "attacked:condition";
        Entity m_entity;

        public Attackable(Entity entity)
        {
            m_entity = entity;
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                attack = ((Params)pars).attack
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (Resistance)ev.actor.m_statManager.GetFile("attacked/res");
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = (ArrayFile)ev.actor.m_statManager.GetFile("attacked/source_res");
            if (sourceRes[ev.attack.source] > ev.attack.power)
                ev.attack.damage = 0;
        }

        static void Armor(Event ev)
        {
            if (ev.attack.pierce < ev.resistance.pierce)
                ev.attack.damage = 0;
            else
                ev.attack.damage = System.Math.Clamp(
                    ev.attack.damage - ev.resistance.armor,
                    ev.resistance.minDamage,
                    ev.resistance.maxDamage);
        }

        static void TakeHit(Event ev)
        {
            System.Console.WriteLine($"Taken {ev.attack.damage} damage");
        }


        public class AttackablenessEvent : CommonEvent
        {
            public AtkCondition attackableness = AtkCondition.ALWAYS;
            public Attacking.Event attackingEvent;
        }

        // TODO: this should get passed the attacker and the info about the attack
        // so Attacking.Event event        
        public AtkCondition GetAttackableness(Attacking.Event attackingEvent)
        {
            var ev = new AttackablenessEvent
            {
                actor = this.m_entity,
                attackingEvent = attackingEvent
            };
            GetChain<AttackablenessEvent>(s_conditionChainName).Pass(ev);
            return ev.attackableness;
        }

        public static ChainPaths<Attackable, Event> Check;
        public static ChainPaths<Attackable, Event> Do;
        public static ChainPaths<Attackable, AttackablenessEvent> Condition;

        static Attackable()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            Check = new ChainPaths<Attackable, Event>(s_checkChainName);
            // this can be cleaned up by using lambdas
            // this way we would eliminate the need of static methods
            // i.e. e => e.actor.beh_Attackable.MethodName(e)
            // or, even better, wrap it in a method Wrap(func, id) and call it as
            // Wrap(func, s_factory.id)
            check.AddHandler(SetResistance, PRIORITY_RANKS.HIGH);
            check.AddHandler(ResistSource, PRIORITY_RANKS.LOW);
            check.AddHandler(Armor, PRIORITY_RANKS.LOW);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            Do = new ChainPaths<Attackable, Event>(s_doChainName);
            _do.AddHandler(TakeHit);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.attacked_do));

            var condition = builder.AddTemplate<AttackablenessEvent>(s_conditionChainName);
            Condition = new ChainPaths<Attackable, AttackablenessEvent>(s_conditionChainName);

            BehaviorFactory<Attackable>.s_builder = builder;

            SetupStats();
        }

    }
}