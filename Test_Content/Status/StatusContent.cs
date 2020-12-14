using Hopper.Core;
using Hopper.Core.Retouchers;
using Hopper.Test_Content.Status.Freeze;

namespace Hopper.Test_Content.Status
{
    public class StatusContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            IceCube.MoveCapturedRetoucher.RegisterSelf(registry);
            FreezeStatus.Status.RegisterSelf(registry);
            IceCube.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            FreezeStatus.Status.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
            IceCube.Factory.AfterPatch(repository);
        }
    }
}