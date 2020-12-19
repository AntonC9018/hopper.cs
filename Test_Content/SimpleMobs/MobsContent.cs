using Hopper.Core;
using Hopper.Core.Registries;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.SimpleMobs
{
    internal class MobsContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Knipper.Factory.RegisterSelf(registry);
            Dummy.Factory.RegisterSelf(registry);
            Skeleton.Factory.RegisterSelf(registry);
            Ghost.TeleportAfterAttackRetoucher.RegisterSelf(registry);
            Ghost.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
        }

        public void PostPatch(PatchArea patchArea)
        {
            Knipper.Factory.PostPatch(patchArea);
            Dummy.Factory.PostPatch(patchArea);
            Skeleton.Factory.PostPatch(patchArea);
            Ghost.Factory.PostPatch(patchArea);
        }
    }
}