using Hopper.Core;
using Hopper.Core.Behaviors.Basic;

namespace Hopper.Test_Content.Floor
{
    public class IceFloor : Entity
    {
        public override Layer Layer => Layer.FLOOR;
        public static EntityFactory<IceFloor> Factory = CreateFactory(SlideStatus.Status);

        public static EntityFactory<IceFloor> CreateFactory(SlideStatus slideStatus)
        {
            return new EntityFactory<IceFloor>()
                .AddBehavior(Attackable.DefaultPreset) // bombs can destroy it
                .AddBehavior(Sliding.Preset(slideStatus));
        }
    }
}