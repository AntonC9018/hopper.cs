using Hopper.Core;

namespace Hopper.Test_Content.Explosion
{
    public class BombContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            BombEntity.Factory.RegisterSelf(registry);
            Bomb.Tinker.RegisterSelf(registry);
            Bomb.Item.RegisterSelf(registry);
            Bomb.Item_x3.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            Explosion.EventPath.Event.Patch(repository);
            Explosion.AtkSource.Patch(repository);
            Explosion.PushSource.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
        }
    }
}