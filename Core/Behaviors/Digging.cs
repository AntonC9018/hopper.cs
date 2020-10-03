using Chains;
using System.Collections.Generic;
using Core.FS;
using Utils.Vector;
using Core.Items.Shovel;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Diggable : Behavior
    {
        public static Attack.Source DigAttackSource;


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
                target.entity.Behaviors.Get<Attackable>().Activate(ev.action, pars);
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
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(DigSetup).TypeHandle);
        }
    }
}