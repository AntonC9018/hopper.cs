using Hopper.Utils.Chains;
using Hopper.Utils;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Targeting;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    /// <summary>
    /// Allows the entity to be attacked.
    /// Note that the entity won't take damage, unless it is also <c>Damageable</c>.
    /// Related update codes: <c>attacked_do</c>.
    /// </summary>
    [DataContract]
    public class Attackable : Behavior, IInitable<Attackness>
    {
        public class Event : ActorEvent
        {
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

        /// <summary> Specifies the criteria used for deciding whether this entity will be targeted by an attack </summary>
        public Attackness m_attackness;
        
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

        /// <returns>
        /// Returns true if the owner of the queried attackable behavior can be attacked by the given attacker
        /// </returns>
        public bool IsAttackable(IWorldSpot attacker)
        {
            // if can be attacked only if next to
            if (m_attackness.Is(Attackness.CAN_BE_ATTACKED_IF_NEXT_TO))
            {
                // returns true if the attacker is next to the entity
                return attacker == null || (attacker.Pos - m_entity.Pos).Abs().ComponentSum() <= 1;
            }
            // if can be attacked by default
            return m_attackness.Is(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT);
        }

        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// Sets the initial resistance stat from stats manager.
        /// </summary>
        public static Handler<Event> SetResistanceHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                ev.resistance = ev.actor.Stats.GetLazy(Attack.Resistance.Path);
            },
            priority = PriorityMapping.Medium + 0x8000
        };

        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// It queries attack source resistance stat, and sets the attack damage to 0 
        /// if the resistance to the source specified by the attack is greater than the attack power.
        /// </summary>
        public static Handler<Event> ResistSourceHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                if (GetSourceResistance(ev) > ev.atkParams.attack.power)
                {
                    ev.atkParams.attack.damage = 0;
                }
            },
            priority = PriorityMapping.Low + 0x8000
        };

        private static int GetSourceResistance(Event ev)
        {
            var sourceRes = ev.actor.Stats.GetLazy(Attack.Source.Resistance.Path);
            return sourceRes[ev.atkParams.attack.sourceId];
        }

        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// If the damage is not already zero, it decreases it by the armor amount.
        /// </summary>
        public static Handler<Event> ArmorHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                if (ev.atkParams.attack.damage > 0)
                {
                    ev.atkParams.attack.damage = Maths.Clamp(
                        ev.atkParams.attack.damage - ev.resistance.armor,
                        ev.resistance.minDamage,
                        ev.resistance.maxDamage);
                }
            },
            priority = PriorityMapping.Low + 0x2000
        };

        /// <summary>
        /// This is one of the default handlers used by the DO chain. 
        /// It activates the <c>Damageable</c> behavior if the attack's pierce is as high as the pierce resistance.
        /// </summary>
        public static Handler<Event> TakeHitHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                // if pierce is high enough, resist the taken damage altogether
                if (ev.resistance.pierce <= ev.atkParams.attack.pierce)
                {
                    ev.actor.Behaviors.TryGet<Damageable>()?.Activate(ev.atkParams.attack.damage);
                }
            },
            priority = PriorityMapping.Low + 0x8000
        };

        /// <summary>
        /// This is one of the default handlers used by the DO chain. 
        /// </summary>
        public static Handler<Event> UpdateHistoryHandler = new Handler<Event>
        {
            handler = Utils.AddHistoryEvent(History.UpdateCode.attacked_do),
            priority = PriorityMapping.Low + 0x2000
        };

        public static readonly ChainPaths<Attackable, Event> Check = new ChainPaths<Attackable, Event>(ChainName.Check);
        public static readonly ChainPaths<Attackable, Event> Do = new ChainPaths<Attackable, Event>(ChainName.Do);

        public static readonly ChainTemplateBuilder DefaultBuilder = 
            new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                    .AddHandler(SetResistanceHandler)
                    .AddHandler(ResistSourceHandler)
                    .AddHandler(ArmorHandler)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(TakeHitHandler)
                    .AddHandler(UpdateHistoryHandler)
                .End();

        /// <summary>
        /// The preset of attackable that uses the default handlers.
        /// The attackness is set to ALWAYS.
        /// </summary>
        public static ConfigurableBehaviorFactory<Attackable, Attackness> DefaultPreset
            => new ConfigurableBehaviorFactory<Attackable, Attackness>(DefaultBuilder, Attackness.ALWAYS);

        /// <summary>
        /// The preset of attackable that uses the default handlers.
        /// It lets you specify the attackness that you wish.
        /// </summary>
        public static ConfigurableBehaviorFactory<Attackable, Attackness> Preset(Attackness attackness)
            => new ConfigurableBehaviorFactory<Attackable, Attackness>(DefaultBuilder, attackness);
    }
}