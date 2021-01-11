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

        public static Handler<Event> ConvertFromMoveHandler = new Handler<Event>
        {
            handler = (Event ev) =>
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

                // @Incomplete in this case you should probably add the bump to the history and stop
                // also this should be done in the do chain
                // the thing is that 0 movement messes up some systmes of the game
                // e.g. listeners on cell's enter and leave events. 
                if (ev.newPos == ev.actor.Pos)
                {
                }
            },
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.High
        };

        public static Handler<Event> DisplaceRemoveHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                ev.actor.RemoveFromGrid();
                ev.actor.Pos = ev.newPos;
            },
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static Handler<Event> DisplaceAddBackHandler = new Handler<Event>
        {
            handler = (Event ev) =>
            {
                ev.actor.ResetInGrid();
            },
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static Handler<Event> UpdateHistoryHandler = new Handler<Event>
        {
            handler = Utils.AddHistoryEvent(History.UpdateCode.displaced_do),
            // @Incomplete hardcode a reasonable priority value 
            priority = (int)PriorityRank.Default
        };

        public static readonly ChainPaths<Displaceable, Event> Check = new ChainPaths<Displaceable, Event>(ChainName.Check);

        public static readonly ChainPaths<Displaceable, Event> Do = new ChainPaths<Displaceable, Event>(ChainName.Do);

        public static readonly ChainTemplateBuilder DefaultBuilder = 
            new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Check)
                    .AddHandler(ConvertFromMoveHandler)
                .AddTemplate<Event>(ChainName.Do)
                    .AddHandler(DisplaceRemoveHandler)
                    .AddHandler(UpdateHistoryHandler)
                    .AddHandler(DisplaceAddBackHandler)
                .End();

        public static ConfigurableBehaviorFactory<Displaceable, Layer> DefaultPreset =>
            new ConfigurableBehaviorFactory<Displaceable, Layer>(DefaultBuilder, ExtendedLayer.BLOCK);
        public static ConfigurableBehaviorFactory<Displaceable, Layer> Preset(Layer layer) =>
            new ConfigurableBehaviorFactory<Displaceable, Layer>(DefaultBuilder, layer);

    }
}