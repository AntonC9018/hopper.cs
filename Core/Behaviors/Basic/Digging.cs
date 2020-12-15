using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
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
            ev.dig = ev.actor.Stats.GetLazy(Dig.Path);
        }

        private static void SetTargets(Event ev)
        {
            if (ev.targets == null)
            {
                ev.targets = new List<Target>();
                if (ev.actor.Inventory != null)
                {
                    var shovel = ev.actor.Inventory.GetContainer(BasicSlots.Shovel)[0];
                    if (shovel != null)
                    {
                        ev.targets.AddRange(shovel.GetTargets(ev.actor, ev.action.direction));
                    }
                }
            }
        }

        private static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                var atkParams = new Attackable.Params(ev.dig.ToAttack(), ev.actor);
                target.entity.Behaviors
                    .Get<Attackable>()
                    .Activate(ev.action.direction, atkParams);
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