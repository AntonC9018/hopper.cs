using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;

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

        private void Init(Config config)
        {
            m_blockLayer = (config == null) ? ExtendedLayer.BLOCK : config.blockLayer;
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

            // TODO: in this case you should probably add the bump to the history and stop
            // also this should be done in the do chain
            // the thing is that 0 movement messes up some systmes of the game
            // e.g. listeners on cell's enter and leave events. 
            if (ev.newPos == ev.actor.Pos)
            {
            }
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


        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Displaceable> Preset =>
            new ConfiglessBehaviorFactory<Displaceable>(DefaultBuilder);

        static Displaceable()
        {
            Check = new ChainPaths<Displaceable, Event>(ChainName.Check);
            Do = new ChainPaths<Displaceable, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(ConvertFromMove, PriorityRanks.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(DisplaceRemove)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.displaced_do))
                .AddHandler(DisplaceAddBack)

                .End();
        }

    }
}