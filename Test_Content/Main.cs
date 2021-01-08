using Hopper.Core;
using Hopper.Core.Mods;
using Hopper.Core.Registries;
using Hopper.Test_Content;
using Hopper.Test_Content.Bind;
using Hopper.Test_Content.Boss;
using Hopper.Test_Content.Explosion;
using Hopper.Test_Content.Floor;
using Hopper.Test_Content.Projectiles;
using Hopper.Test_Content.SimpleMobs;
using Hopper.Test_Content.Status;
using Hopper.Test_Content.Trap;

namespace Hopper.Test_Content
{
    public class TestMod : IMod
    {
        public int Offset => 100;
        public string Name => "test";

        public ISubMod[] subMods = new ISubMod[]
        {
            new BindContent(),
            new BossContent(),
            new BombContent(),
            new FloorContent(),
            new ItemsContent(),
            new MobsContent(),
            new WallsContent(),
            new TrapContent(),
            new StatusContent(),
            new ProjectileContent()
        };

        public void RegisterSelf(ModRegistry registry)
        {
            Laser.EventPath.Event.RegisterSelf(registry);
            Laser.AttackSource.RegisterSelf(registry);
            Laser.PushSource.RegisterSelf(registry);

            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"Registering submod {subMod.GetType().Name}...");
                subMod.RegisterSelf(registry);
            }
        }

        public void PrePatch(PatchArea patchArea)
        {
            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"Pre_Patching submod {subMod.GetType().Name}...");
                subMod.PrePatch(patchArea);
            }
        }

        public void Patch(PatchArea patchArea)
        {
            Laser.EventPath.Event.Patch(patchArea);
            Laser.AttackSource.Patch(patchArea);
            Laser.PushSource.Patch(patchArea);

            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"Patching submod {subMod.GetType().Name}...");
                subMod.Patch(patchArea);
            }
        }

        public void PostPatch(PatchArea patchArea)
        {
            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"Post_Patching submod {subMod.GetType().Name}...");
                subMod.PostPatch(patchArea);
            }
        }
    }
}