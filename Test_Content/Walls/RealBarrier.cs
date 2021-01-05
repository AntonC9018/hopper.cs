using Hopper.Core;

namespace Hopper.Test_Content
{
    public class RealBarrier : Entity
    {
        public override bool IsDirected => true;
        public override Layer Layer => Layer.REAL;

        public static EntityFactory<RealBarrier> Factory;
        public static EntityFactory<RealBarrier> CreateFactory()
        {
            return new EntityFactory<RealBarrier>();
        }
    }
}