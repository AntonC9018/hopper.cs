using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Utils.Vector;

namespace Hopper.Core.History
{
    public interface IUpdateInfo{}
    public class EntityState : IUpdateInfo // possible include some more data
    {
        public TransformSnapshot transformSnapshot;
        public int timeframe;

        public EntityState(Entity entity)
        {
            this.transformSnapshot = entity.GetTransform().GetSnapshot();
            this.timeframe = World.Global.GetNextTimeFrame();
        }

        public EntityState(Transform transform)
        {
            this.transformSnapshot = transform.GetSnapshot();
            this.timeframe = World.Global.GetNextTimeFrame();
        }

        public EntityState(IntVector2 pos, IntVector2 orientation, int timeframe)
        {
            this.transformSnapshot.position = pos;
            this.transformSnapshot.orientation = orientation;
            this.timeframe = timeframe;
        }
    }

    public static class EntityStateHistoryExtensions
    {

    }
}