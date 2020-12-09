using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class Target
    {
        public Entity entity;
        public IntVector2 direction;

        public Target(Entity entity, IntVector2 direction)
        {
            this.entity = entity;
            this.direction = direction;
        }
    }
}