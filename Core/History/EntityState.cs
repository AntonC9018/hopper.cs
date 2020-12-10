using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Core.History
{
    public class EntityState // possible include some more data
    {
        public IntVector2 pos;
        public IntVector2 orientation;
        public int timeframe;

        public EntityState(Entity entity)
        {
            this.pos = entity.Pos;
            this.orientation = entity.Orientation;
            this.timeframe = entity.World.GetNextTimeFrame();
        }

        public EntityState(IWorldSpot spot, IntVector2 orientation)
        {
            this.pos = spot.Pos;
            this.orientation = orientation;
            this.timeframe = spot.World.GetNextTimeFrame();
        }

        public EntityState(IntVector2 pos, IntVector2 orientation, int timeframe)
        {
            this.pos = pos;
            this.orientation = orientation;
            this.timeframe = timeframe;
        }
    }

    public static class EntityStateHistoryExtensions
    {

    }
}