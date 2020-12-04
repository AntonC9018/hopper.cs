using Core.Utils.Vector;

namespace Core.Targeting
{
    public enum AtkCondition
    {
        NEVER = 0,
        ALWAYS = 1,
        SKIP = 2,
        IF_NEXT_TO = 3
    }

    public class AtkTarget
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;
        public Piece piece;
        public Entity targetEntity;

        public AtkTarget(AtkCondition atkCondition, Piece piece, Entity targetEntity)
        {
            this.atkCondition = atkCondition;
            this.piece = piece;
            this.targetEntity = targetEntity;
        }
    }
}