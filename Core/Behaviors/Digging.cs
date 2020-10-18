using Chains;
using System.Collections.Generic;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    [DataContract]
    public class Digging : Behavior, IStandartActivateable
    {
        public static Attack.Source DigAttackSource;

        public class Event : CommonEvent
        {
            public Dig dig;
            public List<Target> targets;
        }

        public bool Activate(Action action)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetDig(Event ev)
        {
            ev.dig = (Dig)ev.actor.StatManager.GetFile("dig");
        }

        static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                var inv = ev.actor.Inventory;
                ev.targets = inv == null
                    ? new List<Target>()
                    : inv.GenerateTargets(ev, Inventory.ShovelSlot);
            }
        }

        static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                target.targetEntity.Behaviors
                    .Get<Attackable>()
                    .Activate(ev.action, ev.dig.ToAttack());
            }
        }

        public static ChainPaths<Digging, Event> Check;
        public static ChainPaths<Digging, Event> Do;
        static Digging()
        {
            Check = new ChainPaths<Digging, Event>(ChainName.Check);
            Do = new ChainPaths<Digging, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetDig, PriorityRanks.High)
                .AddHandler(SetTargets, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Attack)

                .End();

            // _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Digging>.s_builder = builder;
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(DigSetup).TypeHandle);
        }
    }
}