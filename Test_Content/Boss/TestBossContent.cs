using Hopper.Core;
using Hopper.Core.Registries;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class BossContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            TestBoss.TurnToPlayerRetoucher.RegisterSelf(registry);
            TestBoss.Whelp.Factory.RegisterSelf(registry);
            TestBoss.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
        }

        public void PostPatch(PatchArea patchArea)
        {
            TestBoss.Whelp.Factory.PostPatch(patchArea);
            TestBoss.Factory.PostPatch(patchArea);
        }
    }
}