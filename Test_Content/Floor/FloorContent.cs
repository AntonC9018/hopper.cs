using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Floor
{
    public class FloorContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            SlideStatus.Status.RegisterSelf(registry);
            StuckStatus.Status.RegisterSelf(registry);

            IceFloor.Factory.RegisterSelf(registry);
            Water.Factory.RegisterSelf(registry);
            RealBarrier.Factory.RegisterSelf(registry);
            BlockingTrap.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            SlideStatus.Status.Patch(patchArea);
            StuckStatus.Status.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            IceFloor.Factory.PostPatch(patchArea);
            Water.Factory.PostPatch(patchArea);
            RealBarrier.Factory.PostPatch(patchArea);
            BlockingTrap.Factory.PostPatch(patchArea);
        }
    }
}