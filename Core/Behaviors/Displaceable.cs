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
                var cell = ev.actor.GetCellRelative(ev.dir * i);

                if (cell == null || cell.GetEntityFromLayer(ExtendedLayer.BLOCK) != null)
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.dir * i);
        }

        static void Displace(Event ev)
        {
            ev.actor.ResetPosInGrid(ev.newPos);
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
                .AddHandler(Displace)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.displaced_do))

                .End();

            BehaviorFactory<Displaceable>.s_builder = builder;
        }

    }
}