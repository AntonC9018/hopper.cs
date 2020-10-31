using Core;

namespace Test
{
    public class Barrier : Entity
    {
        public override bool IsDirected => true;
        public override Layer Layer => Layer.WALL;
        public static readonly EntityFactory<Barrier> Factory = CreateFactory();

        public static EntityFactory<Barrier> CreateFactory()
        {
            return new EntityFactory<Barrier>();
        }
    }
}