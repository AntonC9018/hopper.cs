using Hopper.Utils.Chains;
using Hopper.Utils;
using System.Runtime.Serialization;
using Hopper.Core.Stat;
using Hopper.Utils.Vector;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Components.Basic
{
    /// <summary>
    /// Allows the entity to be attacked.
    /// Note that the entity won't take damage, unless it is also <c>Damageable</c>.
    /// </summary>
    public partial class Attackable : IBehavior
    {
        public class Context : ActorContext
        {
            public Entity attacker;
            public IntVector2 direction;
            public Attack attack;
            public Attack.Resistance resistance;
            public Attack.Source.Resistance sourceResistance;

            public Context(Entity actor, Entity attacker, Attack attack, IntVector2 direction)
            {
                this.actor = actor;
                this.attacker = attacker;
                this.direction = direction;
                this.attack = attack;
            
                var stats = actor.GetStats();

                resistance = stats.GetLazy(Attack.Resistance.Index);
                sourceResistance = stats.GetLazy(attack.source);
            }

            public void Logic()
            {
                if (attack.damage == 0)
                {
                    return;
                }

                if (sourceResistance.amount > attack.power)
                {
                    attack.damage = 0;
                    return;
                }

                // if pierce is high enough, resist the taken damage altogether
                if (resistance.pierce <= attack.pierce)
                {
                    attack.damage = Maths.Clamp(
                        attack.damage - resistance.armor,
                        resistance.minDamage,
                        resistance.maxDamage
                    );

                    actor.TryBeDamaged(attack.damage);
                }
            } 
        }

        /// <summary> 
        /// Specifies the criteria used for deciding whether this entity will be targeted by an attack 
        /// </summary>
        [Inject] public Attackness _attackness;

        [Chain("Should")] private Chain<Context> _ShouldChain;
        [Chain("After")]  private Chain<Context> _AfterChain;


        // TODO: Add more chains
        [Alias("BeAttacked")]
        public void Activate(Entity actor, Entity attacker, Attack attack, IntVector2 direction)
        {

            var context = new Context(actor, attacker, attack, direction);
            
            if (_ShouldChain.PassWithPropagationChecking(context))
            {
                context.Logic();
                _AfterChain.Pass(context);
            }
        }

        /// <returns>
        /// Returns true if the owner of the queried attackable behavior can be attacked by the given attacker
        /// </returns>
        public bool IsAttackable(Transform transform, Transform attacker_Transform)
        {
            // if can be attacked only if next to
            if (_attackness.HasFlag(Attackness.CAN_BE_ATTACKED_IF_NEXT_TO))
            {
                // returns true if the attacker is next to the entity
                return transform == null 
                    || (transform.position - attacker_Transform.position).Abs().ComponentSum() <= 1;
            }
            // if can be attacked by default
            return _attackness.HasFlag(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT);
        }
    }
}