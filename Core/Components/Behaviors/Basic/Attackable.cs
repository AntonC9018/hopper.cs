using Hopper.Utils.Chains;
using Hopper.Utils;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Targeting;
using Hopper.Core.Stats;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    /// <summary>
    /// Allows the entity to be attacked.
    /// Note that the entity won't take damage, unless it is also <c>Damageable</c>.
    /// Related update codes: <c>attacked_do</c>.
    /// </summary>
    [AutoActivation("BeAttacked")]
    public partial class Attackable : IBehavior
    {
        public class Context : ActorContext
        {
            public Entity attacker;
            public Attack attack;
            public IntVector2 direction;
            [Omit] public Attack.Resistance resistance;
        }

        /// <summary> 
        /// Specifies the criteria used for deciding whether this entity will be targeted by an attack 
        /// </summary>
        [Inject] public Attackness m_attackness;

        /// <returns>
        /// Returns true if the owner of the queried attackable behavior can be attacked by the given attacker
        /// </returns>
        [Export] public bool IsAttackable(
            TransformComponent transform, TransformComponent attacker_Transform)
        {
            // if can be attacked only if next to
            if (m_attackness.Is(Attackness.CAN_BE_ATTACKED_IF_NEXT_TO))
            {
                // returns true if the attacker is next to the entity
                return transform == null 
                    || (transform.position - attacker_Transform.position).Abs().ComponentSum() <= 1;
            }
            // if can be attacked by default
            return m_attackness.Is(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT);
        }

        
        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// Sets the initial resistance stat from stats manager.
        /// </summary>
        [Export] public static void SetResistance(
            Stats.Stats stats, out Attack.Resistance resistance)
        {
            resistance = stats.GetLazy(Attack.Resistance.Path);
            // priority = PriorityMapping.Medium + 0x8000
        }

        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// It queries attack source resistance stat, and sets the attack damage to 0 
        /// if the resistance to the source specified by the attack is greater than the attack power.
        /// </summary>
        [Export] public static void ResistSource(Stats.Stats stats, Attack attack)
        {
            if (stats.GetLazy(Attack.Source.Resistance.Path)[attack.sourceId] > attack.power)
            {
                attack.damage = 0;
            }
            // priority = PriorityMapping.Low + 0x8000
        }

        /// <summary>
        /// This is one of the default handlers used by the CHECK chain. 
        /// If the damage is not already zero, it decreases it by the armor amount.
        /// </summary>
        [Export] public static void Armor(Attack attack, Attack.Resistance resistance)
        {
            if (attack.damage > 0)
            {
                attack.damage = Maths.Clamp(
                    attack.damage - resistance.armor,
                    resistance.minDamage,
                    resistance.maxDamage
                );
            }
            // priority = PriorityMapping.Low + 0x2000
        }

        /// <summary>
        /// This is one of the default handlers used by the DO chain. 
        /// It activates the <c>Damageable</c> behavior if the attack's pierce is as high as the pierce resistance.
        /// </summary>
        [Export] public static void TakeHit(
            Entity actor, 
            Attack attack, 
            Attack.Resistance resistance)
        {
            // if pierce is high enough, resist the taken damage altogether
            if (resistance.pierce <= attack.pierce)
            {
                actor.BeDamaged(attack.damage);
            }
            // priority = PriorityMapping.Low + 0x8000
        }

        public void DefaultPreset()
        {
            _CheckChain.Add(SetResistanceHandler, ResistSourceHandler, ArmorHandler);
            _DoChain   .Add(TakeHitHandler);
        }

        /// <summary>
        /// The preset of attackable that uses the default handlers.
        /// The attackness is set to ALWAYS.
        /// </summary>

        /// <summary>
        /// The preset of attackable that uses the default handlers.
        /// It lets you specify the attackness that you wish.
        /// </summary>
    }
}