using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Displaceable : Behavior, IInitable<Layer>
    {
        public class Event : StandartEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
            public Layer blockLayer;
        }

        public Layer blockLayer;

        public void Init(Layer blockLayer)
        {
            this.blockLayer = blockLayer;
        }

        public bool Activate(IntVector2 dir, Move move)
        {
            var ev = new Event
            {
                actor = m_entity,
                direction = dir,
                move = move,
                blockLayer = blockLayer
            };
            return CheckDoCycle<Event>(ev);
        }

        private static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                if (ev.actor.HasBlockRelative(ev.direction * i, ev.blockLayer))
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.direction * i);

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

        public static ConfigurableBehaviorFactory<Displaceable, Layer> DefaultPreset =>
            new ConfigurableBehaviorFactory<Displaceable, Layer>(DefaultBuilder, ExtendedLayer.BLOCK);
        public static ConfigurableBehaviorFactory<Displaceable, Layer> Preset(Layer layer) =>
            new ConfigurableBehaviorFactory<Displaceable, Layer>(DefaultBuilder, layer);

        static Displaceable()
        {
            Check = new ChainPaths<Displaceable, Event>(ChainName.Check);
            Do = new ChainPaths<Displaceable, Event>(ChainName.Do);

            DefaultBuilder = new ChainTemplateBuilder()

                .AddTemplate<Event>(ChainName.Check)
                .AddHandler(ConvertFromMove, PriorityRank.High)

                .AddTemplate<Event>(ChainName.Do)
                .AddHandler(DisplaceRemove)
                .AddHandler(Utils.AddHistoryEvent(History.UpdateCode.displaced_do))
                .AddHandler(DisplaceAddBack)

                .End();
        }

    }
}