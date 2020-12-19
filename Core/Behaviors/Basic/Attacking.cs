using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using System.Linq;
using Hopper.Utils.Vector;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Attacking : Behavior, IStandartActivateable
    {
        public class Event : StandartEvent
        {
            public List<Target> targets;
            public Attack attack;
            public Push push;
        }

        private static List<Target> GenerateTargetsDefault(Event ev)
            => TargetProvider.SimpleAttack.GetTargets(ev.actor, ev.action.direction).ToList();

        public bool Activate(Action action) => Activate(action, null);
        public bool Activate(Action action, List<Target> targets)
        {
            var ev = new Event
            {
                targets = targets,
                actor = m_entity,
                action = action
            };
            return CheckDoCycle<Event>(ev);
        }

        private static void SetBase(Event ev)
        {
            if (ev.attack == null)
            {
                ev.attack = ev.actor.Stats.GetLazy(Attack.Path);
            }
            if (ev.push == null)
            {
                ev.push = ev.actor.Stats.GetLazy(Push.Path);
            }
        }

        private static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                if (ev.actor.Inventory != null)
                {
                    var weapon = ev.actor.Inventory.GetContainer(BasicSlots.Weapon)[0];
                    if (weapon != null)
                    {
                        // Get targets from weapon, using its target provider
                        // TODO: Save these initial targets at history or something
                        // since we don't want out event to have excessive data. 
                        // It may be useful in some cases, but not yet
                        ev.targets = weapon
                            .GetTargets(ev.actor, ev.action.direction)
                            .ConvertAll(t => new Target(t.entity, t.piece.dir));
                    }
                    else
                    {
                        ev.targets = new List<Target>();
                    }
                }
                else
                {
                    ev.targets = GenerateTargetsDefault(ev);
                }
            }
        }

        private static void ApplyAttack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                System.Console.WriteLine($"Attacking {target.entity}");
                ApplyAttack(target.entity, target.direction, (Attack)ev.attack.Copy(), ev.actor);
            }
        }

        public static bool TryAttackWithConditionCheck(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            if (attacked.Behaviors.Has<Attackable>())
            {
                var attackable = attacked.Behaviors.Get<Attackable>();
                if (attackable.IsAttackable(attacker))
                {
                    attackable.Activate(direction, new Attackable.Params(attack, attacker));
                }
            }
            return false;
        }

        public static bool TryApplyAttack(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            if (attacked.Behaviors.Has<Attackable>())
            {
                return attacked.Behaviors.Get<Attackable>()
                    .Activate(direction, new Attackable.Params(attack, attacker));
            }
            return false;
        }

        public static bool ApplyAttack(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            return attacked.Behaviors.Get<Attackable>()
                .Activate(direction, new Attackable.Params(attack, attacker));
        }

        private static void ApplyPush(Event ev)
        {
            foreach (var target in ev.targets)
            {
                TryApplyPush(target.entity, target.direction, (Push)ev.push.Copy());
            }
        }

        public static void TryApplyPush(Entity attacked, IntVector2 direction, Push push)
        {
            attacked.Behaviors.TryGet<Pushable>()?.Activate(direction, push);
        }

        public static void ApplyPush(Entity attacked, IntVector2 direction, Push push)
        {
            attacked.Behaviors.Get<Pushable>().Activate(direction, push);
        }

        public static readonly ChainPaths<Attacking, Event> Check;
        public static readonly ChainPaths<Attacking, Event> Do;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Attacking> Preset
            => new ConfiglessBehaviorFactory<Attacking>(DefaultBuilder);

        static Attacking()
        {
            Check = new ChainPaths<Attacking, Event>(ChainName.Check);
            Do = new ChainPaths<Attacking, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetTargets, PriorityMapping.Low + 0x8000)
                .AddHandler(SetBase, PriorityMapping.Low + 0x2000)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(ApplyAttack)
                .AddHandler(ApplyPush)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacking_do))

                .End();
        }
    }
}