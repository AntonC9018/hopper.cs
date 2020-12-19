using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Floor
{
    internal class FloorContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Slide.Status.RegisterSelf(registry);
            Stuck.Status.RegisterSelf(registry);

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
            Stuck.Status.Patch(patchArea);
            Stuck.Status.Patch(patchArea);
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