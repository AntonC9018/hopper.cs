using Utils.Vector;

namespace Core.History
{
    public class EntityState // possible include some more data
    {
        public IntVector2 pos;
        public IntVector2 orientation;

        public EntityState(Entity entity)
        {
            pos = entity.Pos;
            orientation = entity.Orientation;
        }
    }
}