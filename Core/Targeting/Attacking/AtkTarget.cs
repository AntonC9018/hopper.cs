namespace Hopper.Core.Targeting
{
    public class AtkTarget
    {
        public Attackness attackness = Attackness.NEVER;
        public Piece piece;
        public Entity entity;

        public AtkTarget(Attackness attackness, Piece piece, Entity targetEntity)
        {
            this.attackness = attackness;
            this.piece = piece;
            this.entity = targetEntity;
        }
    }
}