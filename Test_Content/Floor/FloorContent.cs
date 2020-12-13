using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Floor
{
    public class FloorContent
    {
        public EntityFactory<RealBarrier> RealBarrierFactory;
        public EntityFactory<BlockingTrap> BlockingTrapFactory;
        public EntityFactory<IceFloor> IceFloorFactory;
        public EntityFactory<Water> WaterFactory;

        public SlideStatus SlideStatus;
        public StuckStatus StuckStatus;

        public FloorContent()
        {
            SlideStatus = SlideStatus.Create(1);
            StuckStatus = StuckStatus.Create(1);

            IceFloorFactory = IceFloor.CreateFactory(SlideStatus);
            WaterFactory = Water.CreateFactory();

            RealBarrierFactory = BlockingTrap.CreateBarrierFactory();
            BlockingTrapFactory = BlockingTrap.CreateFactory();
        }

        public void RegisterSelf(Registry registry)
        {
            SlideStatus.RegisterSelf(registry);
            StuckStatus.RegisterSelf(registry);
            IceFloorFactory.RegisterSelf(registry);
            WaterFactory.RegisterSelf(registry);
            RealBarrierFactory.RegisterSelf(registry);
            BlockingTrapFactory.RegisterSelf(registry);
        }
    }
}