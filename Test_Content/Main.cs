using Hopper.Core;
using Hopper.Core.Mods;
using Hopper.Core.Registry;
using Hopper.Test_Content;
using Hopper.Test_Content.Bind;
using Hopper.Test_Content.Boss;
using Hopper.Test_Content.Explosion;
using Hopper.Test_Content.Floor;
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
        };

        public void RegisterSelf(ModSubRegistry registry)
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

        public void Patch(Repository repository)
        {
            Laser.EventPath.Event.Patch(repository);
            Laser.AttackSource.Patch(repository);
            Laser.PushSource.Patch(repository);

            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"Patching submod {subMod.GetType().Name}...");
                subMod.Patch(repository);
            }
        }

        public void AfterPatch(Repository repository)
        {
            foreach (var subMod in subMods)
            {
                System.Console.WriteLine($"AfterPatching submod {subMod.GetType().Name}...");
                subMod.AfterPatch(repository);
            }
        }
    }
}