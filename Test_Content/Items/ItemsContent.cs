using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Core.Registries;
using Hopper.Core.Retouchers;

namespace Hopper.TestContent.Boss
{
    internal class ItemsContent : ISubMod
    {
        public void RegisterSelf(ModRegistry registry)
        {
            Bow.ArrowSource.RegisterSelf(registry);
            Bow.DefaultItem.RegisterSelf(registry);
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