using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Bind
{
    public class BindContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            BindStatuses.StopMove.RegisterSelf(registry);
            BindRetouchers.StopMoveRetoucher.RegisterSelf(registry);
            Spider.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            BindStatuses.StopMove.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            Spider.Factory.PostPatch(patchArea);
        }
    }
}