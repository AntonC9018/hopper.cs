using System.Runtime.Serialization;
using Hopper.Utils.Chains;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Pushable : Behavior
    {
        public class Event : ActorEvent
        {
            public Push push;
            public Push.Resistance resistance;
            public IntVector2 dir;
        }

        public bool Activate(IntVector2 dir, Push push)
        {
            var ev = new Event
            {
                actor = m_entity,
                push = push,
                dir = dir
            };
            return CheckDoCycle<Event>(ev);
        }

        static void SetResistance(Event ev)
        {
            ev.resistance = ev.actor.Stats.GetLazy(Push.Resistance.Path);
        }

        static void ResistSource(Event ev)
        {
            var sourceRes = ev.actor.Stats.GetLazy(Push.Source.Resistance.Path);
            if (sourceRes[ev.push.sourceId] > ev.push.power)
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
            if (ev.push.distance > 0)
            {
                ev.actor.Behaviors.Get<Displaceable>()
                    .Activate(ev.dir, ev.push.ConvertToMove());
            }
        }

        public static readonly ChainPaths<Pushable, Event> Check;
        public static readonly ChainPaths<Pushable, Event> Do;

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Pushable> Preset =>
            new ConfiglessBehaviorFactory<Pushable>(DefaultBuilder);

        static Pushable()
        {
            Check = new ChainPaths<Pushable, Event>(ChainName.Check);
            Do = new ChainPaths<Pushable, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetResistance, PriorityRank.High)
                .AddHandler(ResistSource, PriorityRank.High)
                .AddHandler(Armor, PriorityRank.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(BePushed)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.pushed_do))

                .End();
        }
    }
}