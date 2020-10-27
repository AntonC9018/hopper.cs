using Core.Utils.Vector;

namespace Core.Targeting
{
    public interface ITarget<T, M>
        where T : Target
    {
        void CalculateTargetedEntity(
            TargetEvent<T> ev,
            Cell cell,
            Layer skipLayer,
            Layer targetedLayer);

        void ProcessMeta(M meta);
    }

    public class Target
    {
        public static TargetEvent<T> CreateEvent<T>(
            IWorldSpot entity, IntVector2 dir)
            where T : Target, new()
        {
            return new TargetEvent<T>
            {
                spot = entity,
                dir = dir
            };
        }

        public static TargetEvent<T> CreateEvent<T>(
            StandartEvent commonEvent)
            where T : Target, new()
        {
            return new TargetEvent<T>
            {
                spot = commonEvent.actor,
                dir = commonEvent.action.direction
            };
        }

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