using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;
using Hopper.Core.Targeting;

namespace Hopper.Core.Behaviors.Basic
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
            public Layer blockLayer;
        }

        public class Config
        {
            public Layer blockLayer;

            public Config(Layer blockLayer)
            {
                this.blockLayer = blockLayer;
            }
        }

        private Layer m_blockLayer;

        public void Init(Config config)
        {
            m_blockLayer = config == null ? ExtendedLayer.BLOCK : config.blockLayer;
        }

        public bool Activate(IntVector2 dir, Move move)
        {
            var ev = new Event
            {
                actor = m_entity,
                dir = dir,
                move = move,
                blockLayer = m_blockLayer
            };
            return CheckDoCycle<Event>(ev);
        }

        private static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                if (ev.actor.HasBlockRelative(ev.dir * i, ev.blockLayer))
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.dir * i);
        }

        private static void DisplaceRemove(Event ev)
        {
            ev.actor.RemoveFromGrid();
            ev.actor.Pos = ev.newPos;
        }

        private static void DisplaceAddBack(Event ev)
        {
            ev.actor.ResetInGrid();
        }

        public static readonly ChainPaths<Displaceable, Event> Check;
        public static readonly ChainPaths<Displaceable, Event> Do;

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