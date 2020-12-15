using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class ItemsContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Bow.ArrowSource.RegisterSelf(registry);
            Bow.DefaultBow.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
        }

        public void Patch(PatchArea patchArea)
        {
            Bow.ArrowSource.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
        }
    }
}