using Chains;
using System.Collections.Generic;
using Core.Items;
using Core.Targeting;
using System.Runtime.Serialization;
using Core.Stats.Basic;
using System.Linq;

namespace Core.Behaviors
{
    [DataContract]
    public class Digging : Behavior, IStandartActivateable
    {
        public static Attack.Source DigAttackSource;

        public class Event : StandartEvent
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

        private static void SetDig(Event ev)
        {
            ev.dig = ev.actor.Stats.Get(Dig.Path);
        }

        private static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                var inv = ev.actor.Inventory;
                var shovel = (ModularShovel)inv.GetItemFromSlot(Slot.Shovel);
                if (shovel != null)
                {
                    ev.targets = shovel.GetTargets(ev.actor, ev.action.direction).ToList();
                }
                else
                {
                    ev.targets = new List<Target>();
                }
            }
        }

        private static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                target.targetEntity.Behaviors
                    .Get<Attackable>()
                    .Activate(ev.action.direction,
                        new Attackable.Params(ev.dig.ToAttack(), ev.actor));
            }
        }

        public static readonly ChainPaths<Digging, Event> Check;
        public static readonly ChainPaths<Digging, Event> Do;
        static Digging()
        {
            Check = new ChainPaths<Digging, Event>(ChainName.Check);
            Do = new ChainPaths<Digging, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetDig, PriorityRanks.High)
                .AddHandler(SetTargets, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Attack)

                .End();

            // _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.pushed_do));

            BehaviorFactory<Digging>.s_builder = builder;
            AssureRun(typeof(Dig));
        }
    }
}