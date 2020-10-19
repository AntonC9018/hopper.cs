using Chains;
using Utils.Vector;
using System.Runtime.Serialization;
using Core.Stats.Basic;

namespace Core.Behaviors
{
    [DataContract]
    public class Displaceable : Behavior
    {
        public class Event : CommonEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
        }

        public bool Activate(Action action, Move move)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                move = move
            };
            return CheckDoCycle<Event>(ev);
        }

        static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                var cell = ev.actor.GetCellRelative(ev.action.direction * i);

                if (cell == null || cell.GetEntityFromLayer(Layer.BLOCK) != null)
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.action.direction * i);
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