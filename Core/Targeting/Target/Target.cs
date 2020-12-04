using Core.Utils.Vector;

namespace Core.Targeting
{
    public class Target
    {
        public Entity targetEntity;
        public IntVector2 dir;

        public Target(Entity targetEntity, IntVector2 dir)
        {
            this.targetEntity = targetEntity;
            this.dir = dir;
        }
    }
}