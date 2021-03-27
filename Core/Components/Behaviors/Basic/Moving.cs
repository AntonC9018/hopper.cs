using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Utils.Vector;

namespace Hopper.Core.Components.Basic
{
    [DataContract]
    public class Moving : IBehavior, IStandartActivateable
    {

        public class Context : StandartEvent
        {
            public Move move;
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

        public static Handler<Event> SetBaseHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                if (ev.move == null)
                {
                    ev.move = ev.actor.Stats.GetLazy(Move.Path);
                }
            },
            // @Incomplete hardcode a reasonable priority value
            priority = (int)PriorityRank.High
        };

        public static Handler<Event> DisplaceHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                ev.actor.Behaviors.Get<Displaceable>().Activate(ev.direction, ev.move);
            },
            // @Incomplete hardcode a reasonable priority value
            priority = (int)PriorityRank.Default
        };

        public static Handler<Event> UpdateHistoryHandler = new Handler<Event>
        {
            handler = Utils.AddHistoryEvent(History.UpdateCode.move_do),
            // @Incomplete hardcode a reasonable priority value
            priority = (int)PriorityRank.Default
        };

        public static readonly ChainTemplateBuilder DefaultBuilder = 
            new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                    .AddHandler(SetBaseHandler)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(DisplaceHandler)
                    .AddHandler(UpdateHistoryHandler)
                .End();
    }
}