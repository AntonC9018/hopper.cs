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

        static Attackable()
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
        Chain<Event> chain_checkAttacked;
        Chain<Event> chain_beAttacked;
        Chain<AttackablenessEvent> chain_getAttackableness;
        Entity m_entity;

        public Attackable(Entity entity)
        {
            chain_checkAttacked = (Chain<Event>)entity.m_chains[s_checkChainName];
            chain_beAttacked = (Chain<Event>)entity.m_chains[s_doChainName];
            chain_getAttackableness = (Chain<AttackablenessEvent>)entity.m_chains[s_conditionChainName];
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
            chain_getAttackableness.Pass(ev);
            return ev.attackableness;
        }

        public static void SetupChainTemplates(BehaviorFactory<Attackable> fact)
        {
            var check = fact.AddTemplate<Event>(s_checkChainName);
            var setResitanceHandler = new EvHandler<Event>(SetResistance, PRIORITY_RANKS.HIGH);
            var resistRourceHandler = new EvHandler<Event>(ResistSource, PRIORITY_RANKS.LOW);
            var armorHandler = new EvHandler<Event>(Armor, PRIORITY_RANKS.LOW);
            // this can be cleaned up by using lambdas
            // this way we would eliminate the need of static methods
            // i.e. e => e.actor.beh_Attackable.MethodName(e)
            // or, even better, wrap it in a method Wrap(func, id) and call it as
            // Wrap(func, s_factory.id)
            check.AddHandler(setResitanceHandler);
            check.AddHandler(resistRourceHandler);
            check.AddHandler(armorHandler);

            var _do = fact.AddTemplate<Event>(s_doChainName);
            var takeHitHandler = new EvHandler<Event>(TakeHit);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.attacked_do));
            _do.AddHandler(takeHitHandler);
            _do.AddHandler(addEventHandler);

            var condition = fact.AddTemplate<AttackablenessEvent>(s_conditionChainName);
        }


    }
}