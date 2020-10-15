using System.Collections.Generic;
using Chains;
using Core.Items.Weapon;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Attacking : Behavior, IStandartActivateable
    {
        public class Event : CommonEvent
        {
            public List<Target> targets;
            public Attack attack;
            public Push push;
        }

        static List<Target> GenerateTargetsDefault(Event ev)
        {
            var entity = ev.actor
                .GetCellRelative(ev.action.direction)
                .GetEntityFromLayer(Layer.REAL);

            return entity == null
                ? new List<Target>()
                : new List<Target>
                {
                    new Target { Entity = entity, direction = ev.action.direction }
                };
        }

        static List<Target> GenerateTargets(Event ev)
        {
            var inventory = ev.actor.Inventory;

            if (inventory == null)
                return GenerateTargetsDefault(ev);

            var weapon = (IWeapon)inventory.GetItemFromSlot(Inventory.WeaponSlot);
            return weapon == null ? new List<Target>() : weapon.GetTargets();

        }
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

        static void SetBase(Event ev)
        {
            if (ev.attack == null)
            {
                ev.attack = (Attack)ev.actor.StatManager.GetFile("attack");
            }
            if (ev.push == null)
            {
                ev.push = (Push)ev.actor.StatManager.GetFile("push");
            }
        }

        static void GetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                ev.targets = GenerateTargets(ev);
            }
        }

        static void ApplyAttack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                var attackable = target.Entity.Behaviors.Get<Attackable>();
                // let it throw if this has not been accounted for
                attackable.Activate(action, (Attack)ev.attack.Copy());
            }
        }

        static void ApplyPush(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                Pushable pushable = target.Entity.Behaviors.Get<Pushable>();
                if (pushable != null)
                {
                    pushable.Activate(action, (Push)ev.push.Copy());
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
                .AddHandler(GetTargets, PriorityRanks.Medium)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(ApplyAttack)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.attacking_do))

                .End();

            BehaviorFactory<Attacking>.s_builder = builder;
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AttackSetup).TypeHandle);
        }
    }
}