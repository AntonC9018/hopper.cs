using Hopper.Core;

namespace Test
{
    public class RealBarrier : Entity
    {
        public override bool IsDirected => true;
        public override Layer Layer => Layer.REAL;
        public static readonly EntityFactory<RealBarrier> Factory = CreateFactory();

        public static EntityFactory<RealBarrier> CreateFactory()
        {
            return new EntityFactory<RealBarrier>();
        }
    }
}