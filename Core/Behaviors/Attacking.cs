using System.Collections.Generic;
using Chains;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;
using Core.Stats.Basic;
using System.Linq;
using Core.Utils.Vector;

namespace Core.Behaviors
{
    [DataContract]
    public class Attacking : Behavior, IStandartActivateable
    {
        public class Event : StandartEvent
        {
            public List<AtkTarget> targets;
            public Attack attack;
            public Push push;
        }

        private static List<AtkTarget> GenerateTargetsDefault(Event ev)
        {
            var entity = ev.actor.Cell.GetEntityFromLayer(ev.action.direction, Layer.REAL);

            return entity == null
                ? new List<AtkTarget>()
                : new List<AtkTarget>(1)
                {
                    new AtkTarget(entity, ev.action.direction)
                };
        }

        public bool Activate(Action action) => Activate(action, null);
        public bool Activate(Action action, List<AtkTarget> targets)
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
                ev.attack = ev.actor.Stats.Get(Attack.Path);
            }
            if (ev.push == null)
            {
                ev.push = ev.actor.Stats.Get(Push.Path);
            }
        }

        private static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                var inv = ev.actor.Inventory;
                ev.targets = inv == null
                    ? GenerateTargetsDefault(ev)
                    : inv
                        .GenerateTargets(
                            Target.CreateEvent<AtkTarget>(ev),
                            new Attackable.Params(ev.attack, ev.actor),
                            Slot.Weapon)
                        .ToList();
            }
        }

        private static void ApplyAttack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                ApplyAttack(target.targetEntity, target.piece.dir, (Attack)ev.attack.Copy(), ev.actor);
            }
        }

        // TODO: refactor
        public static bool TryAttackWithConditionCheck(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            if (attacked.Behaviors.Has<Attackable>())
            {
                var attackable = attacked.Behaviors.Get<Attackable>();
                if (attackable.IsAttackable(attack, attacker))
                {
                    attackable.Activate(direction, new Attackable.Params(attack, attacker));
                }
            }
            return false;
        }

        public static bool TryApplyAttack(
            Entity attacked, IntVector2 direction, Attack attack, Entity attacker)
        {
            return attacked.Behaviors.TryGet<Attackable>()
                .Activate(direction, new Attackable.Params(attack, attacker));
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
                TryApplyPush(target.targetEntity, target.piece.dir, (Push)ev.push.Copy());
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

        public static ChainPaths<Attacking, Event> Check;
        public static ChainPaths<Attacking, Event> Do;

        static Attacking()
        {
            Check = new ChainPaths<Attacking, Event>(ChainName.Check);
            Do = new ChainPaths<Attacking, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetBase, PriorityRanks.High)
                .AddHandler(SetTargets, PriorityRanks.Medium)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(ApplyAttack)
                .AddHandler(ApplyPush)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacking_do))

                .End();

            BehaviorFactory<Attacking>.s_builder = builder;
        }
    }
}