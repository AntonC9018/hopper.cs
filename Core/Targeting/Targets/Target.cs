using Utils.Vector;

namespace Core.Targeting
{
    public interface ITarget<in T, in E>
        where T : Target
        where E : TargetEvent<T>
    {
        void CalculateTargetedEntity(E ev, Cell cell);

        Layer TargetedLayer { get; }
        // => Layer.REAL | Layer.MISC | Layer.WALL;
        Layer SkipLayer { get; }// => 0;
    }
    public class Target
    {
        public Piece initialPiece;
        public IntVector2 direction;
        public Entity targetEntity;

        static protected Entity GetEntityDefault(Cell cell, Layer skip, Layer get)
        {
            if (cell.GetEntityFromLayer(skip) == null)
            {
                return cell.GetEntityFromLayer(get);
            }
            return null;
        }
    }
}