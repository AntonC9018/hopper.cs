using Hopper.Core.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class AtkTarget
    {
        public Attackness atkCondition = Attackness.NEVER;
        public Piece piece;
        public Entity targetEntity;

        public AtkTarget(Attackness atkCondition, Piece piece, Entity targetEntity)
        {
            this.atkCondition = atkCondition;
            this.piece = piece;
            this.targetEntity = targetEntity;
        }
    }
}