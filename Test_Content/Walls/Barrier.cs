using Hopper.Core;

namespace Hopper.Test_Content
{
    public class Barrier : Entity
    {
        public override bool IsDirected => true;
        public override Layer Layer => Layer.WALL;

        public static EntityFactory<Barrier> CreateFactory()
        {
            return new EntityFactory<Barrier>();
        }
    }
}