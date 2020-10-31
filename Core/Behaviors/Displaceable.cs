using Chains;
using Core.Utils.Vector;
using System.Runtime.Serialization;
using Core.Stats.Basic;

namespace Core.Behaviors
{
    [DataContract]
    public class Displaceable : Behavior
    {
        public class Event : ActorEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
            public IntVector2 dir;
        }

        public bool Activate(IntVector2 dir, Move move)
        {
            var ev = new Event
            {
                actor = m_entity,
                dir = dir,
                move = move
            };
            return CheckDoCycle<Event>(ev);
        }

        static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                if (ev.actor.HasBlockRelative(ev.dir * i))
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.dir * i);
        }

        static void DisplaceRemove(Event ev)
        {
            ev.actor.RemoveFromGrid();
            ev.actor.Pos = ev.newPos;
        }

        static void DisplaceAddBack(Event ev)
        {
            ev.actor.ResetInGrid();
        }

        public static ChainPaths<Displaceable, Event> Check;
        public static ChainPaths<Displaceable, Event> Do;

        static Displaceable()
        {
            Check = new ChainPaths<Displaceable, Event>(ChainName.Check);
            Do = new ChainPaths<Displaceable, Event>(ChainName.Do);

            var builder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(ConvertFromMove, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(DisplaceRemove)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.displaced_do))
                .AddHandler(DisplaceAddBack)


                .End();

            BehaviorFactory<Displaceable>.s_builder = builder;
        }

    }
}