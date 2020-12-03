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

    public class AtkTarget : Target
    {
        public AtkCondition atkCondition = AtkCondition.NEVER;

        public AtkTarget()
        {
        }

        public AtkTarget(Entity targetEntity, IntVector2 dir)
        {
            this.targetEntity = targetEntity;
            this.piece = new Piece
            {
                index = 0,
                dir = dir
            };
        }
    }
}