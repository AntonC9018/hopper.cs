// this is the class that provides and lets you create and register all
// the kinds contained in the Core mod.
// It provides super essential stuff like
//      1. a base droppedItem factory
//      2. basic retouchers
//      3. stat registering
using Hopper.Core.Items;
using Hopper.Core.Mods;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    public class CoreMod : IMod
    {
        public CoreMod()
        {
        }

        public string Name => "core";
        public int Offset => 0;

        public void RegisterSelf(ModRegistry registry)
        {
            CoreRetouchers.RegisterAll(registry);
            DroppedItem.Factory.RegisterSelf(registry);
            BasicSlots.RegisterSelf(registry);
        }

        public void PrePatch(PatchArea patchArea)
        {
            BasicStats.PrePatch(patchArea);
        }

        public void Patch(PatchArea patchArea)
        {
            BasicSlots.Patch(patchArea);
            BasicStats.Patch(patchArea);
        }

        public void PostPatch(PatchArea patchArea)
        {
            BasicStats.PostPatch(patchArea);
        }
    }
}