using Hopper.Core;

namespace Hopper.Test_Content.Trap
{
    public class TrapContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            Bounce.Source.RegisterSelf(registry);
            BounceTrap.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            Bounce.Source.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
            BounceTrap.Factory.AfterPatch(repository);
        }
    }
}