using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Explosion
{
    internal class BombContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Explosion.EventPath.Event.RegisterSelf(registry);
            BombEntity.Factory = BombEntity.CreateFactory();
            BombEntity.Factory.RegisterSelf(registry);
            Bomb.Tinker.RegisterSelf(registry);
            Bomb.Item.RegisterSelf(registry);
            Bomb.Item_x3.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Explosion.EventPath.Event.Patch(patchArea);
            Explosion.AtkSource.Patch(patchArea);
            Explosion.PushSource.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
        }

    }
}