using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class BossContent
    {
        public EntityFactory<TestBoss> TestBossFactory;
        public EntityFactory<TestBoss.Whelp> WhelpFactory;

        public BossContent(CoreRetouchers retouchers)
        {
            WhelpFactory = TestBoss.Whelp.CreateFactory(retouchers);
            TestBossFactory = TestBoss.CreateFactory(retouchers, WhelpFactory);
        }

        public void RegisterSelf(Registry registry)
        {
            WhelpFactory.RegisterSelf(registry);
            TestBossFactory.RegisterSelf(registry);
        }
    }
}