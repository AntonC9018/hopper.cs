using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Digging : Behavior, IStandartActivateable
    {
        public class Event : StandartEvent
        {
            public Dig dig;
            public List<Target> targets;
        }

        public bool Activate(IntVector2 direction)
        {
            var ev = new Event
            {
                actor = m_entity,
                direction = direction
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
                    if (ev.actor.Inventory.GetShovel(out var shovel))
                    {
                        ev.targets.AddRange(shovel.GetTargets(ev.actor, ev.direction));
                    }
                }
            }
        }

        private static void Attack(Event ev)
        {
            foreach (var target in ev.targets)
            {
                Attacking.TryApplyAttack(target.entity, ev.direction, ev.dig.ToAttack(), ev.actor);
            }
        }

        public static readonly ChainPaths<Digging, Event> Check;
        public static readonly ChainPaths<Digging, Event> Do;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Digging> Preset =>
            new ConfiglessBehaviorFactory<Digging>(DefaultBuilder);

        static Digging()
        {
            Check = new ChainPaths<Digging, Event>(ChainName.Check);
            Do = new ChainPaths<Digging, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetDig, PriorityRank.High)
                .AddHandler(SetTargets, PriorityRank.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Attack)

                .End();
        }
    }
}