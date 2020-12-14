using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class BossContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            TestBoss.TurnToPlayerRetoucher.RegisterSelf(registry);
            TestBoss.Whelp.Factory.RegisterSelf(registry);
            TestBoss.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
        }

        public void AfterPatch(Repository repository)
        {
            TestBoss.Whelp.Factory.AfterPatch(repository);
            TestBoss.Factory.AfterPatch(repository);
        }
    }
}