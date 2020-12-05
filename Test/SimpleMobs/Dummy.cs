using Hopper.Core;
using Hopper.Core.Behaviors;

namespace Test
{
    public class Dummy : Entity
    {
        public static EntityFactory<Dummy> Factory = CreateFactory();

        private static EntityFactory<Dummy> CreateFactory()
        {
            return new EntityFactory<Dummy>()
                .AddBehavior<Attackable>();
        }
    }
}