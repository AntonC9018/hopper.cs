using Hopper.Core;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.SimpleMobs
{
    public class MobsContent
    {
        public EntityFactory<Knipper> KnipperFactory;
        public EntityFactory<Dummy> DummyFactory;
        public EntityFactory<Entity> SkeletonFactory;
        public EntityFactory<Ghost> GhostFactory;
        public Retoucher TeleportAfterAttackRetoucher;

        public MobsContent(CoreRetouchers retouchers)
        {
            KnipperFactory = Knipper.CreateFactory();
            DummyFactory = Dummy.CreateFactory();
            SkeletonFactory = Skeleton.CreateFactory(retouchers);
            TeleportAfterAttackRetoucher = Ghost.CreateTeleportAfterAttack();
            GhostFactory = Ghost.CreateFactory(TeleportAfterAttackRetoucher);
        }

        public void RegisterSelf(Registry registry)
        {
            KnipperFactory.RegisterSelf(registry);
            DummyFactory.RegisterSelf(registry);
            SkeletonFactory.RegisterSelf(registry);
            TeleportAfterAttackRetoucher.RegisterSelf(registry);
            GhostFactory.RegisterSelf(registry);
        }
    }
}