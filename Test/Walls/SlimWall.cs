using Core;

namespace Test
{
    public class Barrier : Entity
    {
        public override Layer Layer => Layer.DIRECTIONAL_WALL;
        public static readonly EntityFactory<Barrier> Factory = CreateFactory();

        public static EntityFactory<Barrier> CreateFactory()
        {
            return new EntityFactory<Barrier>();
        }
    }
}