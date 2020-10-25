using Core;
using Core.Behaviors;

namespace Test
{
    public class IceFloor : Entity
    {
        public override Layer Layer => Layer.FLOOR;

        private static Action SlideAction = new BehaviorAction<Sliding>();

        public static EntityFactory<IceFloor> CreateFactory()
        {
            return new EntityFactory<IceFloor>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo, e => SlideAction))
                .AddBehavior<Sliding>();
        }
    }
}