using System.Collections.Generic;
using Chains;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;
using Core.Stats.Basic;
using System.Linq;

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

        static List<AtkTarget> GenerateTargetsDefault(Event ev)
        {
            var entity = ev.actor
                .GetCellRelative(ev.action.direction)
                .GetEntityFromLayer(Layer.REAL);

            return entity == null
                ? new List<AtkTarget>()
                : new List<AtkTarget>(1)
                {
                    new AtkTarget { targetEntity = entity, direction = ev.action.direction }
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

        static void SetBase(Event ev)
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

        static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                var inv = ev.actor.Inventory;
                ev.targets = inv == null
                    ? GenerateTargetsDefault(ev)
                    : inv
                        .GenerateTargets(
                            Target.CreateEvent<AtkTarget>(ev),
                            ev.attack, Inventory.WeaponSlot)
                        .ToList();
            }
        }

        static void ApplyAttack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var attackable = target.targetEntity.Behaviors.Get<Attackable>();
                // let it throw if this has not been accounted for
                attackable.Activate(ev.action.direction, (Attack)ev.attack.Copy());
            }
        }

        static void ApplyPush(Event ev)
        {
            foreach (var target in ev.targets)
            {
                Pushable pushable = target.targetEntity.Behaviors.Get<Pushable>();
                if (pushable != null)
                {
                    pushable.Activate(target.direction, (Push)ev.push.Copy());
                }
            }
        }


        public static ChainPaths<Attacking, Event> Check;
        public static ChainPaths<Attacking, Event> Do;

        static Attacking()
        {
            Check = new ChainPaths<Attacking, Event>(ChainName.Check);
            Do = new ChainPaths<Attacking, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetBase, PriorityRanks.High)
                .AddHandler(SetTargets, PriorityRanks.Medium)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(ApplyAttack)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacking_do))

                .End();

            BehaviorFactory<Attacking>.s_builder = builder;
        }
    }
}