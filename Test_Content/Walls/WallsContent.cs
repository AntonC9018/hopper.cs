using Hopper.Core;
using Hopper.Core.Registry;

namespace Hopper.Test_Content.Floor
{
    public class WallsContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            Barrier.Factory.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
        }

        public void AfterPatch(Repository repository)
        {
            Barrier.Factory.AfterPatch(repository);
        }
    }
}