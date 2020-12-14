using Hopper.Core;

namespace Hopper.Test_Content.Floor
{
    public class FloorContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            SlideStatus.Status.RegisterSelf(registry);
            StuckStatus.Status.RegisterSelf(registry);

            IceFloor.Factory.RegisterSelf(registry);
            Water.Factory.RegisterSelf(registry);
            RealBarrier.Factory.RegisterSelf(registry);
            BlockingTrap.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            SlideStatus.Status.Patch(repository);
            StuckStatus.Status.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
            IceFloor.Factory.AfterPatch(repository);
            Water.Factory.AfterPatch(repository);
            RealBarrier.Factory.AfterPatch(repository);
            BlockingTrap.Factory.AfterPatch(repository);
        }
    }
}