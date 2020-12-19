using Hopper.Core.Registries;

namespace Hopper.Core.Retouchers
{
    public static class CoreRetouchers
    {
        public static void RegisterAll(ModRegistry registry)
        {
            Equip.RegisterAll(registry);
            Invincibility.RegisterAll(registry);
            Reorient.RegisterAll(registry);
            Skip.RegisterAll(registry);
        }
    }
}