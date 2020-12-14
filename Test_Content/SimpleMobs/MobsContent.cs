using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.SimpleMobs
{
    public class MobsContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            Knipper.Factory.RegisterSelf(registry);
            Dummy.Factory.RegisterSelf(registry);
            Skeleton.Factory.RegisterSelf(registry);
            Ghost.TeleportAfterAttackRetoucher.RegisterSelf(registry);
            Ghost.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
        }

        public void AfterPatch(Repository repository)
        {
            Knipper.Factory.AfterPatch(repository);
            Dummy.Factory.AfterPatch(repository);
            Skeleton.Factory.AfterPatch(repository);
            Ghost.Factory.AfterPatch(repository);
        }
    }
}