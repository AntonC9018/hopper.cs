using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.SimpleMobs
{
    public class Dummy : Entity
    {
        public static EntityFactory<Dummy> CreateFactory()
        {
            return new EntityFactory<Dummy>()
                .AddBehavior<Attackable>();
        }
    }
}