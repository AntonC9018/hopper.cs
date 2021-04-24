using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.TestContent.Projectiles
{
    internal class ProjectileContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Projectile.SimpleFactory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
        }

        public void PostPatch(PatchArea patchArea)
        {
            Projectile.SimpleFactory.PostPatch(patchArea);
        }
    }
}