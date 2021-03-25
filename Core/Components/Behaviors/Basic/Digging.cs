using Hopper.Utils.Chains;
using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Targeting;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components.Basic
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

        public static Handler<Event> SetDigHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                ev.dig = ev.actor.Stats.GetLazy(Dig.Path);
            },
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.High
        };

        public static Handler<Event> SetTargetsHandler = new Handler<Event>
        {
            handler = (Event ev) =>
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
            },
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.High
        };

        public static Handler<Event> AttackHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                foreach (var target in ev.targets)
                {
                    Attacking.TryApplyAttack(target.entity, ev.direction, ev.dig.ToAttack(), ev.actor);
                }
            },
            // @Incomplete: hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static readonly ChainPaths<Digging, Event> Check = new ChainPaths<Digging, Event>(ChainName.Check);
        public static readonly ChainPaths<Digging, Event> Do = new ChainPaths<Digging, Event>(ChainName.Do);

        public static readonly ChainTemplateBuilder DefaultBuilder = 
            new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                    .AddHandler(SetDigHandler)
                    .AddHandler(SetTargetsHandler)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(AttackHandler)
                .End();

        /// <summary>
        /// The preset of attacking that uses the default handlers.
        /// </summary>
        public static ConfiglessBehaviorFactory<Digging> Preset =>
            new ConfiglessBehaviorFactory<Digging>(DefaultBuilder);
    }
}