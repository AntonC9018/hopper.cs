using Hopper.Core;
using Hopper.Core.Registry;

namespace Hopper.Test_Content.Trap
{
    public class TrapContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Bounce.Source.RegisterSelf(registry);
            BounceTrap.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Bounce.Source.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            BounceTrap.Factory.PostPatch(patchArea);
        }
    }
}