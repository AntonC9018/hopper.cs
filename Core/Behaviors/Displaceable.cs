using Chains;
using Utils.Vector;
using System.Runtime.Serialization;

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

        public class Params : ActivationParams
        {
            public Move move;

            public Params(Move move)
            {
                this.move = move;
            }
        }

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                move = pars.move
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
                .AddHandler(Utils.AddHistoryEvent(History.EventCode.displaced_do))

                .End();

            BehaviorFactory<Displaceable>.s_builder = builder;
            AssureRun(typeof(MoveSetup));
        }

    }
}