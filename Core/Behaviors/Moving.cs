using System.Collections.Generic;
using Core.FS;
using Chains;

namespace Core.Behaviors
{
    public class Moving : Behavior, IStandartActivateable
    {

        public class Event : CommonEvent
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

        static void SetBase(Event ev)
        {
            if (ev.move == null)
            {
                // TODO: set stats for move
                ev.move = (Move)ev.actor.StatManager.GetFile("move");
            }
        }

        static void Displace(Event ev)
        {
            var pars = new Displaceable.Params(ev.move);
            ev.actor.Behaviors.Get<Displaceable>().Activate(ev.action, pars);
        }

        public static ChainPaths<Moving, Event> Check;
        public static ChainPaths<Moving, Event> Do;

        static Moving()
        {
            Check = new ChainPaths<Moving, Event>(ChainName.Check);
            Do = new ChainPaths<Moving, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(SetBase, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(Displace)
                .AddHandler(Utils.AddHistoryEvent(History.EventCode.move_do))

                .End();

            BehaviorFactory<Moving>.s_builder = builder;
            AssureRun(typeof(MoveSetup));
        }
    }
}