using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Moving : Behavior, IStandartActivateable
    {

        public class Event : StandartEvent
        {
            public Move move;
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

        private static void SetBase(Event ev)
        {
            if (ev.move == null)
            {
                ev.move = ev.actor.Stats.GetLazy(Move.Path);
            }
        }

        private static void Displace(Event ev)
        {
            ev.actor.Behaviors.Get<Displaceable>().Activate(ev.action.direction, ev.move);
        }

        public static readonly ChainPaths<Moving, Event> Check;
        public static readonly ChainPaths<Moving, Event> Do;


        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Moving> Preset =>
            new ConfiglessBehaviorFactory<Moving>(DefaultBuilder);

        static Moving()
        {
            Check = new ChainPaths<Moving, Event>(ChainName.Check);
            Do = new ChainPaths<Moving, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetBase, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Displace)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.move_do))

                .End();
        }
    }
}