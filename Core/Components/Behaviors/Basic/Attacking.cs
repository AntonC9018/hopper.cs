using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using System.Linq;
using Hopper.Utils.Vector;
using Hopper.Core.Chains;
using Hopper.Core.Predictions;

namespace Hopper.Core.Components.Basic
{
    [DataContract]
    public class Attacking : Behavior, IStandartActivateable, IBehaviorPredictable
    {
        public class Event : StandartEvent
        {
            public List<Target> targets;
            public Attack attack;
            public Push push;
        }

        public bool Activate(IntVector2 direction) => Activate(direction, null);
        public bool Activate(IntVector2 direction, List<Target> targets)
        {
            var ev = new Event
            {
                targets = targets,
                actor = m_entity,
                direction = direction
            };
            return CheckDoCycle<Event>(ev);
        }

        /// <summary>
        /// A helper method. Tries to apply the given attack to the given entity, without checking the attackness. 
        /// </summary>
        public static bool TryApplyAttack(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            if (attacked.Behaviors.Has<Attackable>() && !attacked.IsDead)
            {
                return attacked.Behaviors.Get<Attackable>()
                    .Activate(direction, new Attackable.Params(attack, attacker));
            }
            return false;
        }

        /// <summary>
        /// A helper method. Applies the given attack to the given entity, without checking the attackness. 
        /// </summary>
        public static bool ApplyAttack(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            return attacked.Behaviors.Get<Attackable>()
                .Activate(direction, new Attackable.Params(attack, attacker));
        }

        /// <summary>
        /// A helper method. Tries to apply the given attack to the given entity, having checked the attackness. 
        /// </summary>
        public static bool TryAttackWithAttacknessCheck(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            if (attacked.Behaviors.Has<Attackable>())
            {
                var attackable = attacked.Behaviors.Get<Attackable>();
                if (attackable.IsAttackable(attacker))
                {
                    return attackable.Activate(direction, new Attackable.Params(attack, attacker));
                }
            }
            return false;
        }

        public static void TryApplyPush(Entity attacked, IntVector2 direction, Push push)
        {
            if (!attacked.IsDead && attacked.Behaviors.Has<Pushable>())
            {
                attacked.Behaviors.Get<Pushable>().Activate(direction, push);
            }
        }

        public static void ApplyPush(Entity attacked, IntVector2 direction, Push push)
        {
            attacked.Behaviors.Get<Pushable>().Activate(direction, push);
        }

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It loads the attack and push from the stats manager.  
        /// </summary>
        public static Handler<Event> SetStatsHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                if (ev.attack == null)
                {
                    ev.attack = ev.actor.Stats.GetLazy(Attack.Path);
                }
                if (ev.push == null)
                {
                    ev.push = ev.actor.Stats.GetLazy(Push.Path);
                }
            }, 
            priority = PriorityMapping.Low + 0x8000
        };

        /// <summary>
        /// This is one of the default handlers of the CHECK chain.
        /// It finds targets using the target provider of the currently equipped weapon, if the entity has an inventory.
        /// Otherwise, the default target provider is used.
        /// @Rethink this should really be two separate handlers: one for entities with inventory and one for ones without one. There's no point in being extra abstract.
        /// </summary>
        public static Handler<Event> SetTargetsHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                if (ev.targets == null)
                {
                    if (ev.actor.Inventory != null)
                    {
                        if (ev.actor.Inventory.GetWeapon(out var weapon))
                        {
                            // Get targets from weapon, using its target provider
                            // @Incomplete: Save these initial targets at history or something
                            // since we don't want our event to have excessive data. 
                            // It may be useful in some cases, but not currenlty
                            ev.targets = weapon
                                .GetTargets(ev.actor, ev.direction)
                                .ConvertAll(t => new Target(t.entity, t.piece.dir));
                        }
                        else
                        {
                            ev.targets = new List<Target>();
                        }
                    }
                    else
                    {
                        ev.targets = TargetProvider.SimpleAttack.GetTargets(ev.actor, ev.direction).ToList();
                    }
                }
            },
            priority = PriorityMapping.Low + 0x2000,
        };

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to ATTACK all the targets one by one.
        /// </summary>
        public static Handler<Event> ApplyAttackHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                foreach (var target in ev.targets)
                {
                    TryApplyAttack(target.entity, target.direction, (Attack)ev.attack.Copy(), ev.actor);
                }
            },
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        /// <summary>
        /// This is one of the default handlers of the DO chain.
        /// It tries to PUSH all the targets one by one.
        /// </summary>
        public static Handler<Event> ApplyPushHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                foreach (var target in ev.targets)
                {
                    TryApplyPush(target.entity, target.direction, (Push)ev.push.Copy());
                }
            },
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static Handler<Event> UpdateHistoryHandler = new Handler<Event>
        {
            handler = Utils.AddHistoryEvent(History.UpdateCode.attacking_do),
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        /// <summary>
        /// Returns the positions that would be attacked if the behavior's activate were to be called. 
        /// </summary>
        public IEnumerable<IntVector2> Predict(IntVector2 direction)
        {
            if (m_entity.Inventory != null)
            {
                if (m_entity.Inventory.GetWeapon(out var weapon))
                {
                    foreach (var relativePos in weapon.Pattern.Predict(direction))
                    {
                        yield return m_entity.Pos + relativePos;
                    }
                }
            }
            else
            {
                yield return m_entity.Pos + direction;
            }
        }

        public static readonly ChainPaths<Attacking, Event> Check = new ChainPaths<Attacking, Event>(ChainName.Check);
        public static readonly ChainPaths<Attacking, Event> Do = new ChainPaths<Attacking, Event>(ChainName.Do);

        public static readonly ChainTemplateBuilder DefaultBuilder = 
            new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                    .AddHandler(SetTargetsHandler)
                    .AddHandler(SetStatsHandler)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(ApplyAttackHandler)
                    .AddHandler(ApplyPushHandler)
                    .AddHandler(UpdateHistoryHandler)
                .End();

        /// <summary>
        /// The preset of attacking that uses the default handlers.
        /// </summary>
        public static ConfiglessBehaviorFactory<Attacking> Preset
            => new ConfiglessBehaviorFactory<Attacking>(DefaultBuilder);
    }
}