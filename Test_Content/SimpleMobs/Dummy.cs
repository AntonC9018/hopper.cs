using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.SimpleMobs
{
    public class Dummy : Entity
    {
        public static readonly EntityFactory<Dummy> Factory = CreateFactory();
        public static EntityFactory<Dummy> CreateFactory()
        {
            return new EntityFactory<Dummy>()
                .AddBehavior<Attackable>();
        }
    }
}