using Hopper.Core;
using Hopper.Core.Registries;
using Hopper.Test_Content.Status.Freezing;

namespace Hopper.Test_Content.Status
{
    internal class StatusContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Invincibility.Status.RegisterSelf(registry);
            IceCube.MoveCapturedRetoucher.RegisterSelf(registry);
            Freeze.Status.RegisterSelf(registry);

            IceCube.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Invincibility.Status.Patch(patchArea);
            Freeze.Status.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            IceCube.Factory.PostPatch(patchArea);
        }
    }
}