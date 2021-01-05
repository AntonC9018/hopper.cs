using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Bind
{
    internal class BindContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Bind.StopMoveStatus.RegisterSelf(registry);
            Bind.StopMoveRetoucher.RegisterSelf(registry);
            Spider.Factory = Spider.CreateFactory();
            Spider.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Bind.StopMoveStatus.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            Spider.Factory.PostPatch(patchArea);
        }
    }
}