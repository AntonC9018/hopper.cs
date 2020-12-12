using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content
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