using System.Runtime.Serialization;
using Chains;
using Core.Stats.Basic;

namespace Core.Behaviors
{
    [DataContract]
    public class Pushable : Behavior
    {
        public class Event : CommonEvent
        {
            public Push push;
            public Push.Resistance resistance;
        }

        public bool Activate(Action action, Push push)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                push = push
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = ev.actor.Stats.Get(Push.Resistance.Path);
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = ev.actor.Stats.Get(Push.Source.Resistance.Path);
            if (sourceRes[ev.push.source] > ev.push.power)
            {
                ev.push.distance = 0;
            }
        }

        static void Armor(Event ev)
        {
            if (ev.push.pierce <= ev.resistance.pierce)
            {
                ev.propagate = false;
            }
        }

        static void BePushed(Event ev)
        {
            // TODO: set up properly
            ev.actor.Behaviors.Get<Displaceable>().Activate(ev.action, new Move());
        }

        public static ChainPaths<Pushable, Event> Check;
        public static ChainPaths<Pushable, Event> Do;
        static Pushable()
        {
            Check = new ChainPaths<Pushable, Event>(ChainName.Check);
            Do = new ChainPaths<Pushable, Event>(ChainName.Check);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRanks.High)
                .AddHandler(ResistSource, PriorityRanks.High)
                .AddHandler(Armor, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(BePushed)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.pushed_do))

                .End();

            BehaviorFactory<Pushable>.s_builder = builder;
        }
    }
}