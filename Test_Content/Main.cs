using Hopper.Core;
using Hopper.Core.Mods;
using Hopper.Test_Content;
using Hopper.Test_Content.Bind;
using Hopper.Test_Content.Boss;
using Hopper.Test_Content.Explosion;
using Hopper.Test_Content.Floor;
using Hopper.Test_Content.SimpleMobs;
using Hopper.Test_Content.Trap;

namespace Hopper.Test_Content
{
    public class TestMod : IMod
    {
        public BindContent Bind;
        public BossContent Boss;
        public BombContent Bomb;
        public FloorContent Floor;
        public ItemsContent Item;
        public MobsContent Mob;
        public WallsContent Wall;
        public TrapContent Trap;


        public TestMod(ModsContent mods)
        {
            var coreMod = mods.Get<CoreMod>();
            Bind = new BindContent();
            Bomb = new BombContent(coreMod.Retouchers);
            Boss = new BossContent(coreMod.Retouchers);
            Floor = new FloorContent();
            Item = new ItemsContent();
            Mob = new MobsContent(coreMod.Retouchers);
            Wall = new WallsContent();
            Trap = new TrapContent();
        }
        public void RegisterSelf(Registry registry)
        {
            Laser.EventPath.Event.RegisterSelf(registry);
            Laser.AttackSource.RegisterOn(registry);
            Laser.PushSource.RegisterOn(registry);
            Bind.RegisterSelf(registry);
            Boss.RegisterSelf(registry);
            Bomb.RegisterSelf(registry);
            Floor.RegisterSelf(registry);
            Item.RegisterSelf(registry);
            Mob.RegisterSelf(registry);
            Wall.RegisterSelf(registry);
            Trap.RegisterSelf(registry);
        }
    }
}