using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content
{
    public class IceFloor : Entity
    {
        public override Layer Layer => Layer.FLOOR;

        private static Action SlideAction = new BehaviorAction<Sliding>();

        public static EntityFactory<IceFloor> CreateFactory()
        {
            return new EntityFactory<IceFloor>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo, e => SlideAction))
                .AddBehavior<Attackable>() // bombs can destroy it
                .AddBehavior<Sliding>();
        }
    }
}