using Chains;
using Utils;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Attackable : Behavior
    {
        public class Event : CommonEvent
        {
            public Entity entity;
            public Attack attack;
            public Attack.Resistance resistance;
        }

        public class Params : ActivationParams
        {
            public Attack attack;
        }


        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                attack = pars.attack
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = (Attack.Resistance)ev.actor.StatManager.GetFile("attacked/res");
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = (MapFile)ev.actor.StatManager.GetFile("attacked/source_res");
            if (sourceRes[ev.attack.sourceId] > ev.attack.power)
                ev.attack.damage = 0;
        }

        static void Armor(Event ev)
        {
            if (ev.attack.pierce < ev.resistance.pierce)
                ev.attack.damage = 0;
            else
                ev.attack.damage = Maths.Clamp(
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
            GetChain<AttackablenessEvent>(ChainName.Condition).Pass(ev);
            return ev.attackableness;
        }

        public static ChainPaths<Attackable, Event> Check;
        public static ChainPaths<Attackable, Event> Do;
        public static ChainPaths<Attackable, AttackablenessEvent> Condition;

        static Attackable()
        {
            Do = new ChainPaths<Attackable, Event>(ChainName.Do);
            Check = new ChainPaths<Attackable, Event>(ChainName.Check);
            Condition = new ChainPaths<Attackable, AttackablenessEvent>(ChainName.Condition);

            // this can be cleaned up by using lambdas
            // this way we would eliminate the need of static methods
            // i.e. e => e.actor.beh_Attackable.MethodName(e)
            // or, even better, wrap it in a method Wrap(func, id) and call it as
            // Wrap(func, s_factory.id)
            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRanks.High)
                .AddHandler(ResistSource, PriorityRanks.Low)
                .AddHandler(Armor, PriorityRanks.Low)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(TakeHit)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacked_do))

                .AddTemplate<AttackablenessEvent>(ChainName.Condition)

                .End();

            BehaviorFactory<Attackable>.s_builder = builder;
            AssureRun(typeof(AttackSetup));
        }

    }
}