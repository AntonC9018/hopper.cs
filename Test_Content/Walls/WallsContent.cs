using Hopper.Core;
using Hopper.Core.Registries;

namespace Hopper.Test_Content.Floor
{
    public class WallsContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Barrier.Factory.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Barrier.Factory.PostPatch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            Barrier.Factory.PostPatch(patchArea);
        }
    }
}