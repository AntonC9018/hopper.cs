using Hopper.Utils.Chains;
using Hopper.Utils;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Targeting;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Attackable : Behavior, IInitable<Attackness>
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

        public void Init(Attackness attackness)
        {
            m_attackness = attackness;
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
            ev.resistance = ev.actor.Stats.GetLazy(Attack.Resistance.Path);
        }

        private static void ResistSource(Event ev)
        {
            if (GetSourceResistance(ev) > ev.atkParams.attack.power)
            {
                ev.atkParams.attack.damage = 0;
            }
        }

        private static int GetSourceResistance(Event ev)
        {
            var sourceRes = ev.actor.Stats.GetLazy(Attack.Source.Resistance.Path);
            return sourceRes[ev.atkParams.attack.sourceId];
        }

        private static void Armor(Event ev)
        {
            if (ev.atkParams.attack.damage > 0)
            {
                ev.atkParams.attack.damage = Maths.Clamp(
                    ev.atkParams.attack.damage - ev.resistance.armor,
                    ev.resistance.minDamage,
                    ev.resistance.maxDamage);
            }
        }

        private static void TakeHit(Event ev)
        {
            // if pierce is high enough, resist the taken damage altogether
            if (ev.resistance.pierce > ev.atkParams.attack.pierce)
            {
                ev.actor.Behaviors.TryGet<Damageable>()?.Activate(ev.atkParams.attack.damage);
            }
        }

        public Attackness m_attackness;

        public bool IsAttackable(IWorldSpot attacker)
        {
            return m_attackness == Attackness.ALWAYS || m_attackness == Attackness.IF_NEXT_TO
                && (attacker == null || (attacker.Pos - m_entity.Pos).Abs().ComponentSum() <= 1);
        }

        public static readonly ChainPaths<Attackable, Event> Check;
        public static readonly ChainPaths<Attackable, Event> Do;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfigurableBehaviorFactory<Attackable, Attackness> DefaultPreset
            => new ConfigurableBehaviorFactory<Attackable, Attackness>(DefaultBuilder, Attackness.ALWAYS);
        public static ConfigurableBehaviorFactory<Attackable, Attackness> Preset(Attackness attackness)
            => new ConfigurableBehaviorFactory<Attackable, Attackness>(DefaultBuilder, attackness);

        static Attackable()
        {
            Do = new ChainPaths<Attackable, Event>(ChainName.Do);
            Check = new ChainPaths<Attackable, Event>(ChainName.Check);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRanks.High)
                .AddHandler(ResistSource, PriorityRanks.Low)
                .AddHandler(Armor, PriorityRanks.Low)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(TakeHit)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacked_do))

                .End();
        }

    }
}