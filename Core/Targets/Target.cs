using Vector;

namespace Core.Weapon
{
    public abstract class Target
    {
        public int index;
        public Piece initialPiece;
        public Entity entity;
        public IntVector2 direction;

        public virtual void CalculateCondition(CommonEvent ev)
        { }
    }
}