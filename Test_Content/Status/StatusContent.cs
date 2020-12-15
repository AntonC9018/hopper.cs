using Hopper.Core;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;
using Hopper.Test_Content.Status.Freeze;

namespace Hopper.Test_Content.Status
{
    public class StatusContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            IceCube.MoveCapturedRetoucher.RegisterSelf(registry);
            FreezeStatus.Status.RegisterSelf(registry);
            IceCube.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            FreezeStatus.Status.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            IceCube.Factory.PostPatch(patchArea);
        }
    }
}