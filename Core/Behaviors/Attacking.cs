using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Chains;
using Core.Items.Weapon;
using Core.Items;
using Core.Targeting;

namespace Core.Behaviors
{
    public class Attacking : Behavior, IStandartActivateable
    {

        static void SetupStats()
        {
            Directory baseDir = StatManager.DefaultFS.BaseDir;

            StatFile attackStatFile = new Attack
            {
                source = 0,
                power = 1,
                damage = 1,
                pierce = 1
            };
            StatFile pushStatFile = new Push
            {
                source = 0,
                power = 1,
                distance = 1,
                pierce = 1
            };

            baseDir.AddFile("attack", attackStatFile);
            baseDir.AddFile("push", pushStatFile);
        }

        public class Attack : StatFile
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;
        }

        public class Push : StatFile
        {
            public int source = 0;
            public int power = 1;
            public int distance = 1;
            public int pierce = 1;
        }

        public class Event : CommonEvent
        {
            public List<Target> targets;
            public Attack attack;
            public Push push;
        }

        public class Params : ActivationParams
        {
            public List<Target> targets;
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
                        new Target { entity = entity, direction = ev.action.direction }
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
        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                targets = pars?.targets,
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
                var attackable = target.entity.GetBehavior<Attackable>();
                var pars = new Attackable.Params
                {
                    attack = (Attack)ev.attack.Copy()
                };
                // let it throw if this has not been accounted for
                attackable.Activate(action, pars);
            }
        }

        static void ApplyPush(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var action = ev.action.Copy();
                action.direction = target.direction;
                Pushable pushable = target.entity.GetBehavior<Pushable>();
                if (pushable != null)
                {
                    var pars = new Pushable.Params
                    {
                        push = (Push)ev.push.Copy()
                    };
                    pushable.Activate(action, pars);
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
                .AddHandler(Utils.AddHistoryEvent(History.EventCode.attacking_do))

                .End();

            BehaviorFactory<Attacking>.s_builder = builder;

            SetupStats();
        }
    }
}