using Chains;
using System.Collections.Generic;
using Core.FS;
using Utils.Vector;
using Core.Items.Shovel;
using Core.Items;
using Core.Targeting;

namespace Core.Behaviors
{
    public class Diggable : Behavior
    {
        public static int s_attackSource;
        static void SetupStats()
        {
            s_attackSource = Attackable.RegisterAttackSource("dig");

            Directory baseDir = StatManager.DefaultFS.BaseDir;
            StatFile digStatFile = new Dig
            {
                source = s_attackSource,
                power = 0,
                damage = 1,
                pierce = 10
            };
            baseDir.AddFile("dig", digStatFile);
        }

        public class Dig : StatFile
        {
            public int source;
            public int power;
            public int damage;
            public int pierce;

            public Attacking.Attack ToAttack()
            {
                return new Attacking.Attack
                {
                    source = s_attackSource,
                    power = power,
                    damage = damage,
                    pierce = pierce
                };
            }
        }

        public class Event : CommonEvent
        {
            public Dig dig;
            public List<Target> targets;
        }

        public class Params : ActivationParams
        {
        }

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                // push = pars.push
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetDig(Event ev)
        {
            ev.dig = (Dig)ev.actor.StatManager.GetFile("dig");
        }

        static void GetTargets(Event ev)
        {
            var inventory = ev.actor.Inventory;
            if (inventory == null)
                return;
            var shovel = (IShovel)inventory.GetItemFromSlot(Inventory.ShovelSlot);
            if (shovel == null)
                return;
            ev.targets = shovel.GetTargets();
        }

        static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var pars = new Attackable.Params
                {
                    attack = ev.dig.ToAttack()
                };
                target.entity.GetBehavior<Attackable>().Activate(ev.action, pars);
            }
        }

        public static ChainPaths<Diggable, Event> Check;
        public static ChainPaths<Diggable, Event> Do;
        static Diggable()
        {
            Check = new ChainPaths<Diggable, Event>(ChainName.Check);
            Do = new ChainPaths<Diggable, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetDig, PriorityRanks.High)
                .AddHandler(GetTargets, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Attack)

                .End();

            // _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Diggable>.s_builder = builder;

            SetupStats();
        }
    }
}