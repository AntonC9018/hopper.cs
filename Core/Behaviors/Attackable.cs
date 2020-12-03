using Chains;
using Core.Utils;
using System.Runtime.Serialization;
using Core.Stats.Basic;
using Core.Utils.Vector;
using Core.Targeting;

namespace Core.Behaviors
{
    [DataContract]
    public class Attackable : Behavior
    {
        public class Event : ActorEvent
        {
            public Entity entity;
            public Params atkParams;
            public IntVector2 dir;
            public Attack.Resistance resistance;
        }

        public class Params
        {
            public Attack attack;
            public Entity attacker;

            public Params(Attack attack, Entity attacker)
            {
                this.attack = (Attack)attack.Copy();
                this.attacker = attacker;
            }
        }

        public bool Activate(IntVector2 dir, Params attackableParams)
        {
            var ev = new Event
            {
                actor = m_entity,
                dir = dir,
                atkParams = attackableParams
            };
            return CheckDoCycle<Event>(ev);
        }

        private static void SetResistance(Event ev)
        {
            ev.resistance = ev.actor.Stats.Get(Attack.Resistance.Path);
        }

        private static void ResistSource(Event ev)
        {
            var sourceRes = ev.actor.Stats.Get(Attack.Source.Resistance.Path);
            if (sourceRes[ev.atkParams.attack.sourceId] > ev.atkParams.attack.power)
            {
                ev.atkParams.attack.damage = 0;
            }
        }

        private static void Armor(Event ev)
        {
            if (ev.atkParams.attack.damage == 0)
            {
                return;
            }
            if (ev.resistance.pierce > ev.atkParams.attack.pierce)
            {
                ev.atkParams.attack.damage = 0;
            }
            else
            {
                ev.atkParams.attack.damage = Maths.Clamp(
                    ev.atkParams.attack.damage - ev.resistance.armor,
                    ev.resistance.minDamage,
                    ev.resistance.maxDamage);
            }
        }

        private static void TakeHit(Event ev)
        {
            System.Console.WriteLine($"Attacking {ev.actor.ToString()}");
            ev.actor.Behaviors.TryGet<Damageable>()?.Activate(ev.atkParams.attack.damage);
            System.Console.WriteLine($"Taken {ev.atkParams.attack.damage} damage");
        }


        public class AttackablenessEvent : StandartEvent
        {
            public AtkCondition attackableness = AtkCondition.ALWAYS;
            public Attack attack;
        }

        public AtkCondition GetAtkCondition(Attack atk)
        {
            var ev = new AttackablenessEvent
            {
                actor = this.m_entity,
                attack = atk
            };
            GetChain<AttackablenessEvent>(ChainName.Condition).Pass(ev);
            return ev.attackableness;
        }

        public bool IsAttackable(Attack attack, Entity attacker)
        {
            var condition = GetAtkCondition(attack);
            return condition == AtkCondition.ALWAYS || condition == AtkCondition.IF_NEXT_TO
                && (attacker == null || (attacker.Pos - m_entity.Pos).Abs().ComponentSum() <= 1);
        }

        public static readonly ChainPaths<Attackable, Event> Check;
        public static readonly ChainPaths<Attackable, Event> Do;
        public static readonly ChainPaths<Attackable, AttackablenessEvent> Condition;

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
        }

    }
}