using Core.Utils.Vector;

namespace Core.Targeting
{
    public class Target
    {
        public Piece piece;
        public Entity targetEntity;

        public Target() { }

        public Target(Entity targetEntity, IntVector2 dir)
        {
            this.targetEntity = targetEntity;
            this.piece = new Piece
            {
                index = 0,
                dir = dir
            };
        }

        static protected Entity GetEntityDefault(Cell cell, IntVector2 direction, Layer skip, Layer get)
        {
            if (cell.HasBlock(direction, skip))
            {
                return null;
            }
            return cell.GetEntityFromLayer(direction, get);
        }
    }
}