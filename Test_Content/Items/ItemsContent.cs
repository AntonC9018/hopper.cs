using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class ItemsContent : ISubMod
    {
        public void RegisterSelf(ModSubRegistry registry)
        {
            Bow.ArrowSource.RegisterSelf(registry);
            Bow.DefaultBow.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            Bow.ArrowSource.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
        }
    }
}